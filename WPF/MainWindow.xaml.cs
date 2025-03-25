using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Library;

namespace LibraryApp
{
    public partial class MainWindow : Window
    {
        private DomToolkid _domToolkid;
        private List<Transistor> _transistors = new List<Transistor>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = FilePathTextBox.Text;
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    MessageBox.Show("Please enter a valid file path");
                    return;
                }

                _domToolkid = new DomToolkid(filePath);
                LoadAllTransistors();
                StatusBarText.Text = $"Loaded XML from {filePath}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading XML: {ex.Message}");
            }
        }

        private void SaveXml_Click(object sender, RoutedEventArgs e)
        {
            if (_domToolkid == null)
            {
                MessageBox.Show("No XML document loaded");
                return;
            }

            try
            {
                _domToolkid.saveDoc();
                StatusBarText.Text = "XML saved successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving XML: {ex.Message}");
            }
        }

        private void LoadAllTransistors()
        {
            _transistors.Clear();
            
            // Assuming IDs are sequential and start from 1
            for (int i = 1; i <= 1000; i++) // Adjust the upper limit as needed
            {
                try
                {
                    var transistor = _domToolkid.nodeShow(i);
                    if (transistor != null)
                    {
                        _transistors.Add(transistor);
                    }
                }
                catch
                {
                    // Skip if transistor not found
                }
            }
            
            TransistorsGrid.ItemsSource = _transistors;
        }

        private void SearchTransistor_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(SearchIdTextBox.Text, out int id))
            {
                MessageBox.Show("Please enter a valid ID");
                return;
            }

            try
            {
                var transistor = _domToolkid.nodeShow(id);
                if (transistor != null)
                {
                    // Highlight in grid
                    TransistorsGrid.SelectedItem = transistor;
                    TransistorsGrid.ScrollIntoView(transistor);
                    
                    // Fill edit form
                    FillEditForm(transistor);
                    
                    StatusBarText.Text = $"Found transistor ID: {id}";
                }
                else
                {
                    MessageBox.Show($"Transistor with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching transistor: {ex.Message}");
            }
        }

        private void DeleteTransistor_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(SearchIdTextBox.Text, out int id))
            {
                MessageBox.Show("Please enter a valid ID");
                return;
            }

            try
            {
                _domToolkid.nodeRemove(id);
                LoadAllTransistors();
                ClearEditForm();
                StatusBarText.Text = $"Deleted transistor ID: {id}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting transistor: {ex.Message}");
            }
        }

        private void TransistorsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TransistorsGrid.SelectedItem is Transistor selected)
            {
                FillEditForm(selected);
            }
        }

        private void FillEditForm(Transistor transistor)
        {
            IdTextBox.Text = transistor._id.ToString();
            NameTextBox.Text = transistor._name;
            
            BipolarCheckBox.IsChecked = transistor._types.Contains(TransistorType.BJT);
            FieldCheckBox.IsChecked = transistor._types.Contains(TransistorType.JFET);
            CompositeCheckBox.IsChecked = transistor._types.Contains(TransistorType.Darlington);
            
            VoltageTextBox.Text = transistor._voltage.ToString(CultureInfo.InvariantCulture);
            AmperageTextBox.Text = transistor._amperage.ToString(CultureInfo.InvariantCulture);
            PriceTextBox.Text = transistor._price.ToString(CultureInfo.InvariantCulture);
            CountryTextBox.Text = transistor._country;

        }

        private void ClearEditForm()
        {
            IdTextBox.Text = "";
            NameTextBox.Text = "";
            BipolarCheckBox.IsChecked = false;
            FieldCheckBox.IsChecked = false;
            CompositeCheckBox.IsChecked = false;
            VoltageTextBox.Text = "";
            AmperageTextBox.Text = "";
            PriceTextBox.Text = "";
            CountryTextBox.Text = "";
        }

        private void AddTransistor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var transistor = GetTransistorFromForm();
                if (transistor == null) return;

                _domToolkid.nodeCreate(transistor);
                LoadAllTransistors();
                StatusBarText.Text = $"Added new transistor ID: {transistor._id}";
                StatusTextBlock.Text = "Transistor added successfully";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error adding transistor: {ex.Message}";
            }
        }

        private void UpdateTransistor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(IdTextBox.Text, out int id))
                {
                    MessageBox.Show("Please enter a valid ID");
                    return;
                }

                var transistor = GetTransistorFromForm();
                if (transistor == null) return;

                _domToolkid.nodeUpdate(id, transistor);
                LoadAllTransistors();
                StatusBarText.Text = $"Updated transistor ID: {transistor._id}";
                StatusTextBlock.Text = "Transistor updated successfully";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error updating transistor: {ex.Message}";
            }
        }

        private Transistor GetTransistorFromForm()
        {
            if (!int.TryParse(IdTextBox.Text, out int id))
            {
                MessageBox.Show("Please enter a valid ID");
                return null;
            }

            string name = NameTextBox.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a name");
                return null;
            }

            var types = new List<TransistorType>();
            if (BipolarCheckBox.IsChecked == true) types.Add(TransistorType.BJT);
            if (FieldCheckBox.IsChecked == true) types.Add(TransistorType.JFET);
            if (CompositeCheckBox.IsChecked == true) types.Add(TransistorType.Darlington);
             
            if (types.Count == 0)
            {
                MessageBox.Show("Please select at least one type");
                return null;
            }

            if (!float.TryParse(VoltageTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float voltage))
            {
                MessageBox.Show("Please enter a valid voltage");
                return null;
            }

            if (!float.TryParse(AmperageTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float amperage))
            {
                MessageBox.Show("Please enter a valid amperage");
                return null;
            }

            if (!float.TryParse(PriceTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float price))
            {
                MessageBox.Show("Please enter a valid price");
                return null;
            }

            string country = CountryTextBox.Text;
            if (string.IsNullOrWhiteSpace(country))
            {
                MessageBox.Show("Please enter a country");
                return null;
            }

            return new Transistor(id, name, types, voltage, amperage, price, country);
        }
    }
}