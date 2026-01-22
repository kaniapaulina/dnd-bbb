using System;
using System.Collections.Generic;
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
using Dnd_BBB;
using Dnd_BBB.Core;
using Dnd_BBB.Classes;
using Dnd_BBB.Races;
using Dnd_BBB.Service;
using Dnd_BBB.Exceptions;


namespace DndGUI
{
    /// <summary>
    /// Logika interakcji dla klasy EdycjaPostaciWindow.xaml
    /// </summary>
    public partial class EdycjaPostaciWindow : Window
    {
        private List<Character> loadedCharacters = new List<Character>();

        public EdycjaPostaciWindow()
        {
            InitializeComponent();
            // Spróbuj pobrać listę postaci z pamięci podręcznej aplikacji (Application.Current.Properties)
            TryLoadFromAppProperties();
            BindCharactersToUi();
        }

        // Konstruktor pomocniczy — pozwala przekazać listę bezpośrednio (np. z miejsca gdzie trzymasz cache)
        public EdycjaPostaciWindow(List<Character> characters) : this()
        {
            if (characters != null && characters.Any())
            {
                loadedCharacters = characters;
                BindCharactersToUi();
            }
        }

        // Możliwość odświeżenia listy z zewnątrz
        public void RefreshCharacters(List<Character> characters)
        {
            loadedCharacters = characters ?? new List<Character>();
            BindCharactersToUi();
        }

        private void TryLoadFromAppProperties()
        {
            try
            {
                // preferuj dane z bazy
                var fromDb = PartyRepository.GetAllCharacters();
                if (fromDb != null && fromDb.Any())
                {
                    loadedCharacters = fromDb;
                    if (Application.Current?.Properties != null) Application.Current.Properties["Characters"] = loadedCharacters.ToList();
                    return;
                }

                if (Application.Current?.Properties != null && Application.Current.Properties.Contains("Characters"))
                {
                    if (Application.Current.Properties["Characters"] is List<Character> chars)
                    {
                        loadedCharacters = chars;
                        return;
                    }

                    // jeśli trzymasz inną kolekcję, spróbuj skonwertować
                    if (Application.Current.Properties["Characters"] is IEnumerable<Character> ie)
                    {
                        loadedCharacters = ie.ToList();
                        return;
                    }
                }
            }
            catch
            {
                // ignoruj — jeśli nic nie znajdziemy, zostanie pusta lista
            }
        }

        private void BindCharactersToUi()
        {
            var combo = this.FindName("charactersComboBox") as ComboBox;
            if (combo != null)
            {
                combo.ItemsSource = null;
                combo.ItemsSource = loadedCharacters;
                combo.DisplayMemberPath = "Name";
            }
        }


        // Pozwalamy na nullable e, by można było wywołać z kodu z null (bez ostrzeżeń)
        private void charactersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs? e)
        {
            if ((sender as ComboBox)?.SelectedItem is not Character c) return;


            c.Proficiencies ??= new List<string>();
            c.Spells ??= new List<string>();
            c.Equipment ??= new List<string>();

            if (c.UnitClass != null && (c.Equipment == null || c.Equipment.Count == 0))
            {
                c.UnitClass.AssignStarterPack(c);
            }

            SetTextBoxOrLabel("txtClass", c.UnitClass?.ClassName ?? string.Empty);
            SetTextBoxOrLabel("txtRace", c.UnitRace?.RaceName ?? string.Empty);

            SetContentControl("badgeLvl", c.Level.ToString());
            SetContentControl("badgeHp", c.Hp.ToString());
            SetContentControl("badgeAc", c.Ac.ToString());
            SetContentControl("badgeGold", c.Gold.ToString());

            SetListViewItems("listViewProficiencies", c.Proficiencies ?? new List<string>());
            SetListViewItems("listViewSpells", c.Spells ?? new List<string>());
            SetListViewItems("listViewEquipment", c.Equipment ?? new List<string>());

            SetStatValueControl("lblDextValue", c.Dext);
            SetStatValueControl("lblIntValue", c.Intel);
            SetStatValueControl("lblStrValue", c.Str);
            SetStatValueControl("lblWisValue", c.Wis);
            SetStatValueControl("lblCharValue", c.Charm);
            SetStatValueControl("lblConsValue", c.Cons);

            int CalcModifier(int stat)
            {
                if (c.UnitClass != null) return c.UnitClass.Calc(stat);
                return (int)Math.Floor((stat - 10) / 2.0);
            }

            SetBonusTextBox("txtDextBonus", CalcModifier(c.Dext));
            SetBonusTextBox("txtIntBonus", CalcModifier(c.Intel));
            SetBonusTextBox("txtStrBonus", CalcModifier(c.Str));
            SetBonusTextBox("txtWisBonus", CalcModifier(c.Wis));
            SetBonusTextBox("txtCharBonus", CalcModifier(c.Charm));
            SetBonusTextBox("txtConsBonus", CalcModifier(c.Cons));

            SetBonusTextBox("txtBonusUmiejetnosci", c.ProficiencyBonus);
        }


        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            var combo = this.FindName("charactersComboBox") as ComboBox;
            var c = combo?.SelectedItem as Character;
            if (c == null)
            {
                MessageBox.Show("Wybierz postać, aby zapisać zmiany.", "Brak postaci", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {

                void TrySetStat(string name, Action<int> setter)
                {
                    var ctrl = this.FindName(name);
                    int val;
                    if (ctrl is ContentControl cc && int.TryParse(cc.Content?.ToString(), out val))
                    {
                        setter(val);
                    }
                    else if (ctrl is TextBox tb && int.TryParse(tb.Text.Trim(), out val))
                    {
                        setter(val);
                    }
                    else if (ctrl is Label lbl && int.TryParse(lbl.Content?.ToString(), out val))
                    {
                        setter(val);
                    }
                }

                TrySetStat("lblDextValue", v => c.Dext = v);
                TrySetStat("lblIntValue", v => c.Intel = v);
                TrySetStat("lblStrValue", v => c.Str = v);
                TrySetStat("lblWisValue", v => c.Wis = v);
                TrySetStat("lblCharValue", v => c.Charm = v);
                TrySetStat("lblConsValue", v => c.Cons = v);


                var badgeHp = this.FindName("badgeHp") as ContentControl;
                if (badgeHp != null && int.TryParse(badgeHp.Content?.ToString(), out int hpVal))
                {
                    c.Hp = hpVal;
                }

                var badgeAc = this.FindName("badgeAc") as ContentControl;
                if (badgeAc != null && int.TryParse(badgeAc.Content?.ToString(), out int acVal))
                {
                    c.Ac = acVal;
                }

                var badgeGold = this.FindName("badgeGold") as ContentControl;
                if (badgeGold != null && int.TryParse(badgeGold.Content?.ToString(), out int goldVal))
                {
                    c.Gold = goldVal;
                }


                var badgeLvl = this.FindName("badgeLvl") as ContentControl;
                if (badgeLvl != null && int.TryParse(badgeLvl.Content?.ToString(), out int targetLevel))
                {
                    if (targetLevel > c.Level)
                    {
                        int toGain = targetLevel - c.Level;
                        for (int i = 0; i < toGain; i++)
                        {
                            c.LevelUp();
                        }
                    }
                    else if (targetLevel < c.Level)
                    {
                        MessageBox.Show("Zmniejszanie poziomu nie jest obsługiwane. Poziom pozostał bez zmian.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                // zamiast bezpośredniego zapisu do DB - enqueue do background worker
                await BackgroundDbQueue.Instance.EnqueueSaveCharacterAsync(c);

                // odśwież widok (combo + listy)
                if (combo != null) charactersComboBox_SelectionChanged(combo, null);

                MessageBox.Show("Żądanie zapisu wysłane (zapis w tle).", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisu zmian: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void AddSpell_Click(object sender, RoutedEventArgs e)
        {
            var c = (this.FindName("charactersComboBox") as ComboBox)?.SelectedItem as Character;
            if (c == null)
            {
                MessageBox.Show("Wybierz postać najpierw.", "Brak postaci", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tb = this.FindName("txtNewSpell") as TextBox;
            var val = tb?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(val))
            {
                MessageBox.Show("Podaj nazwę zaklęcia.", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                c.AddSpell(val);
                SetListViewItems("listViewSpells", c.Spells ?? new List<string>());
                if (tb != null) tb.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nie można dodać zaklęcia", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveSpell_Click(object sender, RoutedEventArgs e)
        {

            var c = (this.FindName("charactersComboBox") as ComboBox)?.SelectedItem as Character;
            if (c == null)
            {
                MessageBox.Show("Wybierz postać najpierw.", "Brak postaci", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var lv = this.FindName("listViewSpells") as ListView;
            if (lv == null)
            {
                MessageBox.Show("Brak listy zaklęć.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (lv.SelectedItem is not string sel)
            {
                MessageBox.Show("Wybierz zaklęcie z listy, aby je usunąć.", "Brak zaznaczenia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (c.Spells.Remove(sel))
            {
                SetListViewItems("listViewSpells", c.Spells ?? new List<string>());
            }
            else
            {
                MessageBox.Show("Nie udało się usunąć wybranego zaklęcia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            var c = (this.FindName("charactersComboBox") as ComboBox)?.SelectedItem as Character;
            if (c == null)
            {
                MessageBox.Show("Wybierz postać najpierw.", "Brak postaci", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tb = this.FindName("txtNewEquip") as TextBox;
            var val = tb?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(val))
            {
                MessageBox.Show("Podaj nazwę ekwipunku.", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            c.Equipment.Add(val);
            SetListViewItems("listViewEquipment", c.Equipment ?? new List<string>());
            if (tb != null) tb.Clear();
        }

        private void RemoveEquipment_Click(object sender, RoutedEventArgs e)
        {

            var c = (this.FindName("charactersComboBox") as ComboBox)?.SelectedItem as Character;
            if (c == null)
            {
                MessageBox.Show("Wybierz postać najpierw.", "Brak postaci", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var lv = this.FindName("listViewEquipment") as ListView;
            if (lv == null)
            {
                MessageBox.Show("Brak listy ekwipunku.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (lv.SelectedItem is not string sel)
            {
                MessageBox.Show("Wybierz element ekwipunku z listy, aby go usunąć.", "Brak zaznaczenia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (c.Equipment.Remove(sel))
            {
                SetListViewItems("listViewEquipment", c.Equipment ?? new List<string>());
            }
            else
            {
                MessageBox.Show("Nie udało się usunąć wybranego elementu ekwipunku.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetTextBoxOrLabel(string name, string text)
        {
            var ctrl = this.FindName(name);
            if (ctrl is TextBox tb) tb.Text = text;
            else if (ctrl is Label lbl) lbl.Content = text;
            else if (ctrl is ContentControl cc) cc.Content = text;
        }

        private void SetContentControl(string name, string text)
        {
            var ctrl = this.FindName(name);
            if (ctrl == null) return;


            var valueProp = ctrl.GetType().GetProperty("Value");
            if (valueProp != null && valueProp.CanWrite)
            {
                try
                {
                    var targetType = valueProp.PropertyType;
                    object converted;
                    if (targetType == typeof(string))
                    {
                        converted = text;
                    }
                    else
                    {
                        converted = Convert.ChangeType(text, targetType);
                    }

                    valueProp.SetValue(ctrl, converted);
                    return;
                }
                catch
                {

                }
            }


            if (ctrl is ContentControl cc) cc.Content = text;
        }

        private void SetListViewItems(string name, IEnumerable<string> items)
        {
            var lv = this.FindName(name) as ListView;
            if (lv == null) return;


            if (lv.ItemsSource != null)
                lv.ItemsSource = null;


            if (lv.Items.Count > 0)
                lv.Items.Clear();

            lv.ItemsSource = items ?? new List<string>();
        }

        private void SetStatValueControl(string controlName, int value)
        {
            var ctrl = this.FindName(controlName);
            if (ctrl is ContentControl cc) cc.Content = value.ToString();
            else if (ctrl is TextBox tb) tb.Text = value.ToString();
            else if (ctrl is Label lbl) lbl.Content = value.ToString();
        }

        private void SetBonusTextBox(string controlName, int modifier)
        {
            var ctrl = this.FindName(controlName) as TextBox;
            if (ctrl == null) return;
            ctrl.Text = modifier >= 0 ? $"+{modifier}" : modifier.ToString();
        }

        // Przywrócony przycisk "Zmień statystyki" — otwiera okno UpdateStatsWindow.
        // Jeśli wybrana jest postać, przekazujemy ją do okna; po zamknięciu odświeżamy widok.
        private void ZmianaStatsButton_Click(object sender, RoutedEventArgs e)
        {
            var combo = this.FindName("charactersComboBox") as ComboBox;
            var selected = combo?.SelectedItem as Character;

            if (selected != null)
            {
                var wnd = new UpdateStatsWindow(selected)
                {
                    Owner = this
                };
                wnd.ShowDialog();

                // odśwież aktualny widok po możliwej edycji statów
                if (combo != null) charactersComboBox_SelectionChanged(combo, null);
            }
            else
            {
                var wnd = new UpdateStatsWindow()
                {
                    Owner = this
                };
                wnd.ShowDialog();
            }
        }

        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}

