using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace AshNowModManager.Views
{
    public partial class MainWindow : Window
    {
        private string? _gamePath;
        private ObservableCollection<string> _mods = new();

        public MainWindow()
        {
            InitializeComponent();

            // Assigner l'ItemsSource dès le début si ModsList est non-null
            if (ModsList != null)
                ModsList.ItemsSource = _mods;
        }

        private async void OnChooseFolderClick(object? sender, RoutedEventArgs e)
        {
            var folder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Game Folder",
                AllowMultiple = false
            });

            if (folder?.Any() == true)
            {
                _gamePath = folder.First().Path.LocalPath;
                Log($"Selected game folder: {_gamePath}");

                try
                {
                    // Récupération des dossiers dans le chemin du jeu
                    var mods = Directory.GetDirectories(_gamePath)
                        .Where(d => !Path.GetFileName(d).StartsWith('.'))
                        .Select(Path.GetFileName)
                        .Where(name => !string.IsNullOrEmpty(name))  // filtrer noms null ou vides
                        .ToArray();

                    _mods.Clear();

                    // Ajout sécurisé dans ObservableCollection (mod non-null garanti)
                    foreach (var mod in mods)
                    {
                        if (mod != null) // sécurité supplémentaire même si filtré avant
                            _mods.Add(mod);
                    }

                    Log($"Found {_mods.Count} mod(s).");
                }
                catch (Exception ex)
                {
                    Log($"Error reading mods: {ex.Message}");
                }
            }
            else
            {
                Log("No folder selected.");
            }
        }

        private void OnInstallModClick(object? sender, RoutedEventArgs e)
        {
            Log("Install Mod button clicked — not yet implemented.");
        }

        private void Log(string message)
        {
            if (LogOutput != null)
                LogOutput.Text += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
        }
    }
}
