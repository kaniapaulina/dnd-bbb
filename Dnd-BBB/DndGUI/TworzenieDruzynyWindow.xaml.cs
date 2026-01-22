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
    /// Okno tworzenia nowej drużyny. Pozwala na wybór postaci z bazy 
    /// i grupowanie ich pod wspólną nazwą zespołu.
    /// </summary>
    public partial class TworzenieDruzynyWindow : Window
    {
        private ObservableCollection<Character> partyMembers = new ObservableCollection<Character>();

        public TworzenieDruzynyWindow()
        {
            InitializeComponent();
            listBoxCharacters.ItemsSource = partyMembers;
            listBoxCharacters.DisplayMemberPath = "Name";
        }

        private void DodajButton_Click(object sender, RoutedEventArgs e)
        {
            string name = txtNazwaPostaci.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name)) return;

            var hero = AppCache.Characters.FirstOrDefault(ch => ch.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (hero == null)
            {
                MessageBox.Show("Nie znaleziono takiej postaci w bazie.");
                return;
            }

            if (partyMembers.Any(p => p.Name == hero.Name))
            {
                MessageBox.Show("Ta postać jest już w Twojej drużynie.");
                return;
            }

            partyMembers.Add(hero);
            txtNazwaPostaci.Clear();
        }

        private void ZapiszDruzyny_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNazwa.Text))
            {
                MessageBox.Show("Podaj nazwę drużyny!");
                return;
            }

            var party = new Party(txtNazwa.Text) { PartyMembers = partyMembers.ToList() };

            AppCache.Parties.Add(party);
            party.SaveToDb(party); 

            AppCache.SyncAll();
            this.Close();
        }
    }
}
