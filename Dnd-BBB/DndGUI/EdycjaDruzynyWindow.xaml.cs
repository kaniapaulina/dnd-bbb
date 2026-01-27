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

        public EdycjaDruzynyWindow()
        {
            InitializeComponent();
            BindUi();

            var parties = Party.ReadFromDb();
            var characters = Party.ReadAllCharactersFromDb();

            Application.Current.Properties["Parties"] = new System.Collections.ObjectModel.ObservableCollection<Party>(parties);
            Application.Current.Properties["Characters"] = characters;
        }

        private void BindUi()
        {
            comboBoxParties.ItemsSource = AppCache.Parties;
            comboBoxParties.DisplayMemberPath = "PartyName";
            comboBoxAvailableCharacters.ItemsSource = AppCache.Characters;
            listBoxMembers.ItemsSource = displayedMembers;
        }


        public void RefreshPartiesFromCache() => BindUi();
        private void RefreshList()
        {
            displayedMembers.Clear();
            foreach (var m in currentParty.PartyMembers) displayedMembers.Add(m);
        }

        private void comboBoxParties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxParties.SelectedItem is not Party selected) return;

            currentParty = new Party(selected.PartyName) { PartyMembers = selected.PartyMembers.ToList() };
            txtPartyName.Text = currentParty.PartyName;
            RefreshList();
        }

        private void btnAddMember_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxAvailableCharacters.SelectedItem is Character hero)
            {
                try
                {
                    currentParty.AddMember(hero);
                    RefreshList();
                    comboBoxAvailableCharacters.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Proszę najpierw wybrać postać z listy.");
            }
        }

        private void btnRemoveMember_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxMembers.SelectedItem is Character c)
            {
                currentParty.DeleteMember(c.Name);
                RefreshList();
            }
        }

        private void btnSortByHp_Click(object sender, RoutedEventArgs e) { currentParty.SortByHp(); RefreshList(); }
        private void btnSortByName_Click(object sender, RoutedEventArgs e) { currentParty.SortByName(); RefreshList(); }
        private void btnSortByStr_Click(object sender, RoutedEventArgs e) { currentParty.SortByStr(); RefreshList(); }
        private void btnSortByDext_Click(object sender, RoutedEventArgs e) { currentParty.SortByDext(); RefreshList(); }

        private void btnSaveCopyJson_Click(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DnD_Exports");
            Directory.CreateDirectory(path);
            string file = Path.Combine(path, $"{currentParty.PartyName}.json");

            StorageService.SavePartyJSON(file, currentParty);
            MessageBox.Show($"Wyeksportowano do: {file}");
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            currentParty.PartyName = txtPartyName.Text?.Trim();
            currentParty.SaveToDb(currentParty); // Zapis do bazy danych

            var idx = AppCache.Parties.ToList().FindIndex(p => p.PartyName == currentParty.PartyName);
            if (idx >= 0) AppCache.Parties[idx] = currentParty;

            MessageBox.Show("Zmiany w drużynie zostały zapisane.");
        }
    }
}
