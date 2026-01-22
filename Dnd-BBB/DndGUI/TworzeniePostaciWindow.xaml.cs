using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using Dnd_BBB.Core;
using Dnd_BBB.Classes;
using Dnd_BBB.Races;
using Dnd_BBB.Service;
using Dnd_BBB.Exceptions;

namespace DndGUI
{
    /// <summary>
    /// Logika interakcji dla klasy TworzeniePostaciWindow.xaml
    /// </summary>
    public partial class TworzeniePostaciWindow : Window
    {
        private Character character;
        private bool rollClicked = false;

        public TworzeniePostaciWindow()
        {
            InitializeComponent();
            character = new Character();

            if (this.FindName("txtKlasaPostaci") is TextBox tb)
            {
                tb.TextChanged += TxtKlasaPostaci_TextChanged;
            }

            UpdateSpellControls();
        }

        private void TxtKlasaPostaci_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                var className = tb.Text?.Trim() ?? string.Empty;
                character.UnitClass = CreateUnitClassByName(className);
                UpdateSpellControls();
            }
        }

        private UnitClass? CreateUnitClassByName(string name) => name switch
        {
            "Bard" => new Bard(),
            "Barbarian" => new Barbarian(),
            "Cleric" => new Cleric(),
            "Druid" => new Druid(),
            "Fighter" => new Fighter(),
            "Monk" => new Monk(),
            "Paladin" => new Paladin(),
            "Ranger" => new Ranger(),
            "Rogue" => new Rogue(),
            "Sorcerer" => new Sorcerer(),
            "Warlock" => new Warlock(),
            "Wizard" => new Wizard(),
            _ => null
        };

        private void UpdateSpellControls()
        {
            bool canCast = character.UnitClass?.Spell ?? false;

            if (this.FindName("txtSpell1") is TextBox s1) s1.IsEnabled = canCast;
            if (this.FindName("txtSpell2") is TextBox s2) s2.IsEnabled = canCast;
            if (this.FindName("txtSpell3") is TextBox s3) s3.IsEnabled = canCast;
            if (this.FindName("txtSpell4") is TextBox s4) s4.IsEnabled = canCast;

            if (!canCast)
            {
                if (this.FindName("txtSpell1") is TextBox s10) s10.Text = string.Empty;
                if (this.FindName("txtSpell2") is TextBox s20) s20.Text = string.Empty;
                if (this.FindName("txtSpell3") is TextBox s30) s30.Text = string.Empty;
                if (this.FindName("txtSpell4") is TextBox s40) s40.Text = string.Empty;
            }
        }

        private void RollButton_Click(object sender, RoutedEventArgs e)
        {
            if (rollClicked) return;
            rollClicked = true;
            if (sender is Button btn) btn.IsEnabled = false;

            character.UnitClass = CreateUnitClassByName((this.FindName("txtKlasaPostaci") as TextBox)?.Text?.Trim() ?? string.Empty);

            switch ((this.FindName("txtRasaPostaci") as TextBox)?.Text?.Trim())
            {
                case "Human": character.UnitRace = new Human(); break;
                case "Elf": character.UnitRace = new Elf(); break;
                case "Dwarf": character.UnitRace = new Dwarf(); break;
                case "Halfling": character.UnitRace = new Halfling(); break;
                case "Dragonborn": character.UnitRace = new Dragonborn(); break;
                case "Gnome": character.UnitRace = new Gnome(); break;
                case "Half-Orc": character.UnitRace = new Half_Orc(); break;
                case "Half-Elf": character.UnitRace = new Half_Elf(); break;
                default: MessageBox.Show("Nieznana rasa postaci!"); break;
            }

            if (character.UnitClass != null && character.UnitRace != null)
            {
                character.UnitClass.AssignStats(character);

                losStr.Text = character.Str.ToString();
                losDex.Text = character.Dext.ToString();
                losInt.Text = character.Intel.ToString();
                losWis.Text = character.Wis.ToString();
                losChar.Text = character.Charm.ToString();
                losConst.Text = character.Cons.ToString();

                UpdateSpellControls();
            }
            else
            {
                losStr.Text = "0";
                losDex.Text = "0";
                losInt.Text = "0";
                losWis.Text = "0";
                losChar.Text = "0";
                losConst.Text = "0";
            }
        }
        //poczeka tu i zapisze postać w bazie
        private async void ZapiszButton_Click(Object sender, RoutedEventArgs e)
        {
            character.Name = txtNazwaPostaci.Text?.Trim() ?? character.Name;
            character.AddProficiencies(txtUmiejetnosc1.Text, txtUmiejetnosc2.Text, txtUmiejetnosc3.Text);

            try
            {
                if (!string.IsNullOrWhiteSpace(txtSpell1.Text))
                {
                    if (character.UnitClass?.Spell ?? false) character.AddSpell(txtSpell1.Text);
                    else MessageBox.Show("Ta klasa nie może mieć zaklęć.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (!string.IsNullOrWhiteSpace(txtSpell2.Text))
                {
                    if (character.UnitClass?.Spell ?? false) character.AddSpell(txtSpell2.Text);
                    else MessageBox.Show("Ta klasa nie może mieć zaklęć.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (!string.IsNullOrWhiteSpace(txtSpell3.Text))
                {
                    if (character.UnitClass?.Spell ?? false) character.AddSpell(txtSpell3.Text);
                    else MessageBox.Show("Ta klasa nie może mieć zaklęć.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (!string.IsNullOrWhiteSpace(txtSpell4.Text))
                {
                    if (character.UnitClass?.Spell ?? false) character.AddSpell(txtSpell4.Text);
                    else MessageBox.Show("Ta klasa nie może mieć zaklęć.S", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd przy dodawaniu zaklęć: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (int.TryParse(losStr.Text, out var strValue)) character.Str = strValue;
            if (int.TryParse(losDex.Text, out var dexValue)) character.Dext = dexValue;
            if (int.TryParse(losInt.Text, out var intValue)) character.Intel = intValue;
            if (int.TryParse(losWis.Text, out var wisValue)) character.Wis = wisValue;
            if (int.TryParse(losChar.Text, out var charValue)) character.Charm = charValue;
            if (int.TryParse(losConst.Text, out var conValue)) character.Cons = conValue;

            try
            {
                // Przekazujemy zapis do backendu 
                BackgroundDbQueue.Instance.EnqueueSaveCharacterAsync(character);

                MessageBox.Show("Żądanie zapisu wysłane. Okno zostanie zamknięte.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wysyłania żądania zapisu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
