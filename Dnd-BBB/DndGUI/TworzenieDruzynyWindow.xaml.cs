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
using Dnd_BBB.Service;
using System.IO;

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

            try
            {
                // enqueue zapisu do backendu zamiast bezpośredniego SaveParty
                BackgroundDbQueue.Instance.EnqueueSavePartyAsync(party);

                // zaktualizuj cache aplikacji optymistycznie
                if (Application.Current?.Properties != null)
                {
                    if (Application.Current.Properties.Contains("Parties") && Application.Current.Properties["Parties"] is ObservableCollection<Party> oc)
                    {
                        var idx = oc.ToList().FindIndex(p => p.PartyName == party.PartyName);
                        if (idx >= 0) oc[idx] = party;
                        else oc.Add(party);
                        Application.Current.Properties["Parties"] = oc;
                    }
                    else
                    {
                        Application.Current.Properties["Parties"] = new ObservableCollection<Party>(new[] { party });
                    }
                }

                MessageBox.Show("Żądanie zapisu drużyny wysłane (zapis w tle).", "OK", MessageBoxButton.OK, MessageBoxImage.Information);

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
                MessageBox.Show($"Błąd zapisu do bazy: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Close();
        }
    }
}
