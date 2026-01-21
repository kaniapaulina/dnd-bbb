using Dnd_BBB.Core;
using Dnd_BBB.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
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
//using System.Windows.Shapes;

namespace DndGUI
{
    /// <summary>
    /// Logika interakcji dla klasy EdycjaDruzynyWindow.xaml
    /// </summary>
    public partial class EdycjaDruzynyWindow : Window
    {
        private Party currentParty = new Party();
        private ObservableCollection<Character> displayedMembers = new ObservableCollection<Character>();
        private ObservableCollection<Party> cachedParties = new ObservableCollection<Party>();
        private List<Character> cachedCharacters = new List<Character>();

        public EdycjaDruzynyWindow()
        {
            InitializeComponent();
            LoadCachedParties();
            LoadCachedCharacters();
            BindUi();

            this.Activated += EdycjaDruzynyWindow_Activated;
        }

        private void EdycjaDruzynyWindow_Activated(object? sender, EventArgs e)
        {
            LoadCachedParties();
            RefreshPartiesCombo();
        }

        private void LoadCachedParties()
        {
            try
            {
                if (Application.Current?.Properties != null && Application.Current.Properties.Contains("Parties"))
                {
                    // preferuj ObservableCollection jeśli jest
                    if (Application.Current.Properties["Parties"] is ObservableCollection<Party> oc)
                    {
                        cachedParties = oc;
                        return;
                    }

                    if (Application.Current.Properties["Parties"] is List<Party> list)
                    {
                        cachedParties = new ObservableCollection<Party>(list);
                        // zaktualizuj globalnie na ObservableCollection, żeby pozostałe okna korzystały z tego samego źródła
                        Application.Current.Properties["Parties"] = cachedParties;
                        return;
                    }

                    if (Application.Current.Properties["Parties"] is IEnumerable<Party> ie)
                    {
                        cachedParties = new ObservableCollection<Party>(ie.ToList());
                        Application.Current.Properties["Parties"] = cachedParties;
                        return;
                    }
                }

                cachedParties = new ObservableCollection<Party>();
                Application.Current.Properties["Parties"] = cachedParties;
            }
            catch
            {
                cachedParties = new ObservableCollection<Party>();
            }
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

                cachedCharacters = new List<Character>();
            }
            catch
            {
                cachedCharacters = new List<Character>();
            }
        }

        private void BindUi()
        {

            comboBoxParties.ItemsSource = cachedParties;
            comboBoxParties.DisplayMemberPath = "PartyName";


            cachedParties.CollectionChanged -= CachedParties_CollectionChanged;
            cachedParties.CollectionChanged += CachedParties_CollectionChanged;

            listBoxMembers.ItemsSource = displayedMembers;
            RefreshDisplayedMembers();

        }

        private void CachedParties_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {

            RefreshPartiesCombo();
        }

        private void RefreshPartiesCombo()
        {
            try
            {
                // ItemsSource już ustawione na cachedParties; upewnij się, że zachowujemy wybór
                if (!string.IsNullOrWhiteSpace(currentParty?.PartyName))
                {
                    var sel = cachedParties.FirstOrDefault(p => p.PartyName == currentParty.PartyName);
                    if (sel != null) comboBoxParties.SelectedItem = sel;
                }
            }
            catch
            {
            }
        }

        public void RefreshPartiesFromCache()
        {
            LoadCachedParties();
            BindUi();
        }

        private void comboBoxParties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox)?.SelectedItem is not Party selected) return;

            // Załaduj kopię wybranej drużyny (by nie modyfikować cache bezpośrednio)
            currentParty = new Party(selected.PartyName)
            {
                PartyMembers = selected.PartyMembers?.ToList() ?? new List<Character>()
            };

            txtPartyName.Text = currentParty.PartyName;
            RefreshDisplayedMembers();
        }

        private void btnAddMember_Click(object sender, RoutedEventArgs e)
        {
            var name = txtMemberName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Podaj nazwę członka.", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cachedCharacters == null || cachedCharacters.Count == 0) LoadCachedCharacters();

            var existing = cachedCharacters.FirstOrDefault(ch => string.Equals(ch.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                MessageBox.Show("Postaci o takiej nazwie nie znaleziono na liście postaci.", "Brak postaci", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                currentParty.AddMember(existing);
                RefreshDisplayedMembers();
                txtMemberName.Clear();
                txtMemberName.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można dodać członka: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveMember_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxMembers.SelectedItem is not Character c)
            {
                MessageBox.Show("Wybierz członka do usunięcia.", "Brak wyboru", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            currentParty.DeleteMember(c.Name);
            RefreshDisplayedMembers();
        }

        private void btnSortByName_Click(object sender, RoutedEventArgs e)
        {
            currentParty.SortByName();
            RefreshDisplayedMembers();
        }

        private void btnSortByHp_Click(object sender, RoutedEventArgs e)
        {
            currentParty.SortByHp();
            RefreshDisplayedMembers();
        }

        private void btnSortByStr_Click(object sender, RoutedEventArgs e)
        {
            currentParty.SortByStr();
            RefreshDisplayedMembers();
        }

        private void btnSortByDext_Click(object sender, RoutedEventArgs e)
        {
            currentParty.SortByDext();
            RefreshDisplayedMembers();
        }

        private void btnSaveCopyJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DnDPartyExports");
                Directory.CreateDirectory(folder);

                var safeName = string.IsNullOrWhiteSpace(txtPartyName.Text) ? "Party" : txtPartyName.Text;
                var fileName = $"{SanitizeFileName(safeName)}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var path = Path.Combine(folder, fileName);

                StorageService.SavePartyJSON(path, currentParty);

                MessageBox.Show($"Zapisano kopię drużyny do:\n{path}", "Zapisano", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu JSON: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            currentParty.PartyName = txtPartyName.Text?.Trim();

            try
            {
                if (Application.Current?.Properties != null)
                {
                    if (!Application.Current.Properties.Contains("Parties"))
                    {
                        Application.Current.Properties["Parties"] = new List<Party>();
                    }

                    if (Application.Current.Properties["Parties"] is List<Party> list)
                    {
                        var selectedInCombo = comboBoxParties.SelectedItem as Party;
                        int idx = -1;
                        if (selectedInCombo != null)
                        {
                            idx = list.FindIndex(p => p.PartyName == selectedInCombo.PartyName);
                        }
                        else
                        {
                            idx = list.FindIndex(p => p.PartyName == currentParty.PartyName);
                        }

                        if (idx >= 0) list[idx] = currentParty;
                        else list.Add(currentParty);
                    }
                }


                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DnDPartyExports");
                Directory.CreateDirectory(folder);
                var file = Path.Combine(folder, $"{SanitizeFileName(currentParty.PartyName ?? "Party")}.json");
                StorageService.SavePartyJSON(file, currentParty);


                LoadCachedParties();
                RefreshPartiesCombo();

                MessageBox.Show("Zapisano zmiany drużyny do JSON.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu zmian: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshDisplayedMembers()
        {
            displayedMembers.Clear();
            foreach (var m in currentParty.PartyMembers ?? Enumerable.Empty<Character>())
            {
                displayedMembers.Add(m);
            }
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var ch in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(ch, '_');
            }

            return string.IsNullOrWhiteSpace(name) ? "Party" : name;
        }

    }
}
