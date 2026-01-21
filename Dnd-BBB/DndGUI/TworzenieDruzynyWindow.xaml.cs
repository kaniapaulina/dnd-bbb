using Dnd_BBB.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DndGUI
{
    /// <summary>
    /// Logika interakcji dla klasy TworzenieDruzynyWindow.xaml
    /// </summary>
    public partial class TworzenieDruzynyWindow : Window
    {
        private ObservableCollection<Character> partyMembers = new ObservableCollection<Character>();
        private List<Character> cachedCharacters = new List<Character>();

        public TworzenieDruzynyWindow()
        {
            InitializeComponent();
            listBoxCharacters.ItemsSource = partyMembers;
            listBoxCharacters.DisplayMemberPath = "Name";

            LoadCachedCharacters();
        }

        private void LoadCachedCharacters()
        {
            try
            {
                if (Application.Current?.Properties != null && Application.Current.Properties.Contains("Characters"))
                {
                    if (Application.Current.Properties["Characters"] is List<Character> chars)
                    {
                        cachedCharacters = chars;
                        return;
                    }

                    if (Application.Current.Properties["Characters"] is IEnumerable<Character> ie)
                    {
                        cachedCharacters = ie.ToList();
                        return;
                    }
                }

                // brak cache -> pusta lista
                cachedCharacters = new List<Character>();
            }
            catch
            {
                cachedCharacters = new List<Character>();
            }
        }

        private void DodajButton_Click(object sender, RoutedEventArgs e)
        {
            var name = txtNazwaPostaci.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Podaj imię członka drużyny.", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // upewnij się, że mamy aktualny cache
            if (cachedCharacters == null || cachedCharacters.Count == 0)
            {
                LoadCachedCharacters();
            }

            // znajdź istniejący obiekt Character w cache (case-insensitive)
            var existing = cachedCharacters.FirstOrDefault(ch => string.Equals(ch.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                MessageBox.Show("Nie znaleziono postaci o takiej nazwie w pamięci podręcznej aplikacji. Dodawanie możliwe tylko z istniejących postaci.", "Brak postaci", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // sprawdź czy już na liście
            if (partyMembers.Any(p => string.Equals(p.Name, existing.Name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ta postać jest już na liście drużyny.", "Duplikat", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNazwaPostaci.Clear();
                return;
            }

            partyMembers.Add(existing);
            txtNazwaPostaci.Clear();
            txtNazwaPostaci.Focus();
        }

        private void ZapiszDruzyny_Click(object sender, RoutedEventArgs e)
        {
            var partyName = txtNazwa.Text?.Trim();
            if (string.IsNullOrWhiteSpace(partyName))
            {
                MessageBox.Show("Podaj nazwę drużyny.", "Brak nazwy", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (partyMembers.Count == 0)
            {
                var res = MessageBox.Show("Drużyna jest pusta. Czy mimo to zapisać?", "Pusta drużyna", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res != MessageBoxResult.Yes) return;
            }

            var party = new Party(partyName)
            {
                PartyMembers = partyMembers.ToList()
            };

            // Zapisz do cache aplikacji (Application.Current.Properties["Parties"]) jako ObservableCollection
            try
            {
                if (Application.Current?.Properties != null)
                {
                    // utwórz ObservableCollection jeśli brak
                    if (!Application.Current.Properties.Contains("Parties"))
                    {
                        Application.Current.Properties["Parties"] = new ObservableCollection<Party>();
                    }

                    // obsłuż różne typy, ale preferuj ObservableCollection
                    if (Application.Current.Properties["Parties"] is ObservableCollection<Party> coll)
                    {
                        var existing = coll.FirstOrDefault(p => p.PartyName == party.PartyName);
                        if (existing != null)
                        {
                            // zastąp element — zachowujemy indeks, żeby powiadomić UI
                            var idx = coll.IndexOf(existing);
                            coll[idx] = party;
                        }
                        else
                        {
                            coll.Add(party);
                        }
                    }
                    else if (Application.Current.Properties["Parties"] is List<Party> list)
                    {
                        var idx = list.FindIndex(p => p.PartyName == party.PartyName);
                        if (idx >= 0) list[idx] = party;
                        else list.Add(party);

                        // zamień listę na ObservableCollection, żeby UI mogło automatycznie reagować
                        var newColl = new ObservableCollection<Party>(list);
                        Application.Current.Properties["Parties"] = newColl;
                    }
                    else
                    {
                        // inne typy -> nadpisz ObservableCollection
                        Application.Current.Properties["Parties"] = new ObservableCollection<Party> { party };
                    }
                }

                MessageBox.Show("Zapisano drużynę.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);


                foreach (Window w in Application.Current.Windows)
                {
                    if (w is EdycjaDruzynyWindow ed)
                    {
                        ed.RefreshPartiesFromCache();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Close();
        }
    }
}
