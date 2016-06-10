﻿using Hardcodet.Wpf.TaskbarNotification;
using Aurora.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using Aurora.EffectsEngine;
using Aurora.Utils;
using Aurora.Settings;
using System.IO;
using Aurora.Controls;

namespace Aurora
{
    /// <summary>
    /// Interaction logic for ConfigUI.xaml
    /// </summary>
    public partial class ConfigUI : Window
    {
        Settings.Control_Settings settings_control = new Settings.Control_Settings();
        Profiles.Desktop.Control_Desktop desktop_control = new Profiles.Desktop.Control_Desktop();
        Profiles.Dota_2.Control_Dota2 dota2_control = new Profiles.Dota_2.Control_Dota2();
        Profiles.CSGO.Control_CSGO csgo_control = new Profiles.CSGO.Control_CSGO();
        Profiles.GTA5.Control_GTA5 gta5_control = new Profiles.GTA5.Control_GTA5();
        Profiles.RocketLeague.Control_RocketLeague rocketleague_control = new Profiles.RocketLeague.Control_RocketLeague();
        Profiles.Overwatch.Control_Overwatch overwatch_control = new Profiles.Overwatch.Control_Overwatch();
        Profiles.Payday_2.Control_PD2 payday2_control = new Profiles.Payday_2.Control_PD2();
        Profiles.TheDivision.Control_TheDivision division_control = new Profiles.TheDivision.Control_TheDivision();
        Profiles.LeagueOfLegends.Control_LoL lol_control = new Profiles.LeagueOfLegends.Control_LoL();
        Profiles.HotlineMiami.Control_HM hotline_control = new Profiles.HotlineMiami.Control_HM();
        Profiles.TheTalosPrinciple.Control_TalosPrinciple talosprinciple_control = new Profiles.TheTalosPrinciple.Control_TalosPrinciple();


        EffectColor desktop_color_scheme = new EffectColor(0, 0, 0);
        EffectColor dota2_color_scheme = new EffectColor(102, 0, 0);
        EffectColor csgo_color_scheme = new EffectColor(151, 79, 37);
        EffectColor gta5_color_scheme = new EffectColor(20, 100, 0);
        EffectColor rocketleague_color_scheme = new EffectColor(0, 80, 170);

        EffectColor transition_color = new EffectColor();
        EffectColor current_color = new EffectColor();

        private float transitionamount = 0.0f;

        private FrameworkElement selected_item = null;

        private bool settingsloaded = false;
        private bool shownHiddenMessage = false;

        private PreviewType saved_preview = PreviewType.Desktop;
        private string saved_preview_key = "";

        private Timer virtual_keyboard_timer;
        private TextBlock last_selected_key;
        private Stopwatch recording_stopwatch = new Stopwatch();

        LayerEditor layer_editor = new LayerEditor();

        public ConfigUI()
        {
            InitializeComponent();

            GenerateProfileStack();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!settingsloaded)
            {
                virtual_keyboard_timer = new Timer(100);
                virtual_keyboard_timer.Elapsed += new ElapsedEventHandler(virtual_keyboard_timer_Tick);
                virtual_keyboard_timer.Start();

                settingsloaded = true;
            }

            this.keyboard_record_message.Visibility = Visibility.Hidden;
            this.content_grid.Children.Clear();
            this.content_grid.Children.Add(desktop_control);
            current_color = desktop_color_scheme;
            bg_grid.Background = new SolidColorBrush(Color.FromRgb(desktop_color_scheme.Red, desktop_color_scheme.Green, desktop_color_scheme.Blue));

            List<KeyboardKey> layout = Global.kbLayout.GetLayout();
            double layout_height = 0;
            double layout_width = 0;

            double max_height = this.keyboard_grid.Height;
            double max_width = this.keyboard_grid.Width;
            double cornerRadius = 5;
            double current_height = 0;
            double current_width = 0;
            bool isFirstInRow = true;

            foreach (KeyboardKey key in layout)
            {
                double keyMargin_Left = key.margin_left;
                double keyMargin_Top = (isFirstInRow ? 0 : key.margin_top);

                Border keyBorder = new Border();
                keyBorder.CornerRadius = new CornerRadius(cornerRadius);
                keyBorder.Width = key.width;
                keyBorder.Height = key.height;
                keyBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                keyBorder.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                keyBorder.Margin = new Thickness(current_width + keyMargin_Left, current_height + keyMargin_Top, 0, 0);
                keyBorder.Visibility = System.Windows.Visibility.Visible;
                keyBorder.BorderThickness = new Thickness(1.5);
                keyBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
                keyBorder.Background = new SolidColorBrush(Color.FromArgb(255, 25, 25, 25));
                keyBorder.IsEnabled = key.enabled;
                keyBorder.MouseDown += keyboard_grid_pressed;
                keyBorder.MouseMove += keyboard_grid_moved;
                keyBorder.IsHitTestVisible = true;

                if (!key.enabled)
                {
                    ToolTipService.SetShowOnDisabled(keyBorder, true);
                    keyBorder.ToolTip = new ToolTip { Content = "Changes to this key are not supported" };
                }

                TextBlock keyCap = new TextBlock();
                keyCap.Text = key.visualName;
                keyCap.Tag = key.tag;
                keyCap.FontSize = key.font_size;
                keyCap.FontWeight = FontWeights.Bold;
                keyCap.FontFamily = new FontFamily("Calibri");
                keyCap.TextAlignment = TextAlignment.Center;
                keyCap.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                keyCap.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                keyCap.Margin = new Thickness(0);
                keyCap.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                keyCap.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                keyCap.Visibility = System.Windows.Visibility.Visible;
                keyCap.IsHitTestVisible = true;

                keyBorder.Child = keyCap;

                this.keyboard_grid.Children.Add(keyBorder);
                isFirstInRow = false;

                if (key.width + keyMargin_Left > 0)
                    current_width += key.width + keyMargin_Left;

                if(keyMargin_Top > 0)
                    current_height += keyMargin_Top;


                if (layout_width < current_width)
                    layout_width = current_width;

                if (key.line_break)
                {
                    current_height += 37;
                    current_width = 0;
                    //isFirstInRow = true;
                }

                if (layout_height < current_height)
                    layout_height = current_height;
            }

            //Update size
            if (max_width < layout_width)
            {
                this.keyboard_grid.Width = layout_width;
                this.MaxWidth += layout_width - max_width;
                this.Width = this.MaxWidth;
            }

            if (max_height < layout_height)
            {
                this.keyboard_grid.Height = layout_height;
                this.MaxHeight += layout_height - max_height;
                this.Height = this.MaxHeight;
            }

            keyboard_grid.Children.Add(layer_editor);

            Effects.grid_height = (float)this.keyboard_grid.Height;
            Effects.grid_width = (float)this.keyboard_grid.Width;

            this.UpdateLayout();
        }

        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private void virtual_keyboard_timer_Tick(object sender, EventArgs e)
        {
            if (!ApplicationIsActivated())
                return;

            Dispatcher.Invoke(
                        () =>
                        {
                            if (transitionamount <= 1.0f)
                            {
                                transition_color.BlendColors(current_color, transitionamount += 0.07f);

                                bg_grid.Background = new SolidColorBrush(Color.FromRgb(transition_color.Red, transition_color.Green, transition_color.Blue));
                                bg_grid.UpdateLayout();
                            }


                            Dictionary<Devices.DeviceKeys, System.Drawing.Color> keylights = new Dictionary<Devices.DeviceKeys, System.Drawing.Color>();

                            if (Global.geh.GetPreview() != PreviewType.None)
                                keylights = Global.effengine.GetKeyboardLights();

                            Border[] keys = this.keyboard_grid.Children.OfType<Border>().ToArray();


                            foreach (var child in keys)
                            {
                                if (child is Border &&
                                    (child as Border).Child is TextBlock &&
                                    ((child as Border).Child as TextBlock).Tag is Devices.DeviceKeys
                                    )
                                {
                                    if (keylights.ContainsKey((Devices.DeviceKeys)((child as Border).Child as TextBlock).Tag))
                                    {
                                        ((child as Border).Child as TextBlock).Foreground = new SolidColorBrush(Utils.ColorUtils.DrawingColorToMediaColor(keylights[(Devices.DeviceKeys)((child as Border).Child as TextBlock).Tag]));
                                    }

                                    if (Global.key_recorder.HasRecorded((Devices.DeviceKeys)((child as Border).Child as TextBlock).Tag))
                                        (child as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)255, (byte)0, (byte)(Math.Min(Math.Pow(Math.Cos((double)(Utils.Time.GetMilliSeconds() / 1000.0) * Math.PI) + 0.05, 2.0), 1.0) * 255), (byte)0));
                                    else
                                    {
                                        if ((child as Border).IsEnabled)
                                        {
                                            (child as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)255, (byte)30, (byte)30, (byte)30));
                                        }
                                        else
                                        {
                                            (child as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 100, 100, 100));
                                            (child as Border).BorderThickness = new Thickness(0);
                                        }
                                    }
                                }

                            }//);

                            if (Global.key_recorder.IsRecording())
                                this.keyboard_record_message.Visibility = System.Windows.Visibility.Visible;
                            else
                                this.keyboard_record_message.Visibility = System.Windows.Visibility.Hidden;

                        });
        }

        ////Misc

        private void virtualkeyboard_key_selected(TextBlock key)
        {
            if (key.Tag is Devices.DeviceKeys)
            {
                //Multi key
                if (Global.key_recorder.IsSingleKey())
                {
                    Global.key_recorder.AddKey((Devices.DeviceKeys)(key.Tag));
                    Global.key_recorder.StopRecording();
                }
                else
                {
                    if (Global.key_recorder.HasRecorded((Devices.DeviceKeys)(key.Tag)))
                        Global.key_recorder.RemoveKey((Devices.DeviceKeys)(key.Tag));
                    else
                        Global.key_recorder.AddKey((Devices.DeviceKeys)(key.Tag));
                    last_selected_key = key;
                }
            }
        }

        private void keyboard_grid_pressed(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border && (sender as Border).Child != null && (sender as Border).Child is TextBlock)
            {
                virtualkeyboard_key_selected((sender as Border).Child as TextBlock);
            }
        }

        private void keyboard_grid_moved(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Border && (sender as Border).Child != null && (sender as Border).Child is TextBlock && last_selected_key != ((sender as Border).Child as TextBlock))
            {
                virtualkeyboard_key_selected((sender as Border).Child as TextBlock);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void trayicon_menu_quit_Click(object sender, RoutedEventArgs e)
        {
            virtual_keyboard_timer.Stop();
            trayicon.Visibility = System.Windows.Visibility.Hidden;

            Application.Current.Shutdown();
        }

        private void trayicon_menu_settings_Click(object sender, RoutedEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.Show();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Program.isSilent)
            {
                this.Visibility = System.Windows.Visibility.Hidden;
                this.WindowStyle = WindowStyle.None;
                this.ShowInTaskbar = false;
                Hide();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!shownHiddenMessage)
            {
                trayicon.ShowBalloonTip("Aurora", "This program is now hidden in the tray.", BalloonIcon.None);
                shownHiddenMessage = true;
            }

            Global.geh.SetPreview(PreviewType.None);

            //Hide Window
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Windows.Threading.DispatcherOperationCallback)delegate (object o)
            {
                WindowStyle = WindowStyle.None;
                Hide();
                return null;
            }, null);
            //Do not close application
            e.Cancel = true;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Global.geh.SetPreview(saved_preview, saved_preview_key);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            saved_preview = Global.geh.GetPreview();
            saved_preview_key = Global.geh.GetPreviewProfileKey();
            Global.geh.SetPreview(PreviewType.None);
        }

        private void GenerateProfileStack()
        {
            selected_item = null;
            this.profiles_stack.Children.Clear();

            Image profile_desktop = new Image();
            profile_desktop.Tag = desktop_control;
            profile_desktop.Source = new BitmapImage(new Uri(@"Resources/desktop_icon.png", UriKind.Relative));
            profile_desktop.ToolTip = "Desktop Settings";
            profile_desktop.Margin = new Thickness(0, 5, 0, 0);
            profile_desktop.MouseDown += ProfileImage_MouseDown;
            this.profiles_stack.Children.Add(profile_desktop);

            //Included Game Profiles
            Image profile_dota2 = new Image();
            profile_dota2.Tag = dota2_control;
            profile_dota2.Source = new BitmapImage(new Uri(@"Resources/dota2_64x64.png", UriKind.Relative));
            profile_dota2.ToolTip = "Dota 2 Settings";
            profile_dota2.Margin = new Thickness(0, 5, 0, 0);
            profile_dota2.MouseDown += ProfileImage_MouseDown;
            this.profiles_stack.Children.Add(profile_dota2);

            Image profile_csgo = new Image();
            profile_csgo.Tag = csgo_control;
            profile_csgo.Source = new BitmapImage(new Uri(@"Resources/csgo_64x64.png", UriKind.Relative));
            profile_csgo.ToolTip = "CS:GO Settings";
            profile_csgo.Margin = new Thickness(0, 5, 0, 0);
            profile_csgo.MouseDown += ProfileImage_MouseDown;
            this.profiles_stack.Children.Add(profile_csgo);

            Image profile_gta5 = new Image();
            profile_gta5.Tag = gta5_control;
            profile_gta5.Source = new BitmapImage(new Uri(@"Resources/gta5_64x64.png", UriKind.Relative));
            profile_gta5.ToolTip = "GTA 5 Settings";
            profile_gta5.Margin = new Thickness(0, 5, 0, 0);
            profile_gta5.MouseDown += ProfileImage_MouseDown;
            this.profiles_stack.Children.Add(profile_gta5);

            Image profile_rocketleague = new Image();
            profile_rocketleague.Tag = rocketleague_control;
            profile_rocketleague.Source = new BitmapImage(new Uri(@"Resources/rocketleague_256x256.png", UriKind.Relative));
            profile_rocketleague.ToolTip = "Rocket League Settings";
            profile_rocketleague.Margin = new Thickness(0, 5, 0, 0);
            profile_rocketleague.MouseDown += ProfileImage_MouseDown;
            this.profiles_stack.Children.Add(profile_rocketleague);

            Image profile_overwatch = new Image();
            profile_overwatch.Tag = overwatch_control;
            profile_overwatch.Source = new BitmapImage(new Uri(@"Resources/overwatch_icon.png", UriKind.Relative));
            profile_overwatch.ToolTip = "Overwatch Settings";
            profile_overwatch.Margin = new Thickness(0, 5, 0, 0);
            profile_overwatch.MouseDown += ProfileImage_MouseDown;
            this.profiles_stack.Children.Add(profile_overwatch);
            
            Image profile_payday2 = new Image();
            profile_payday2.Tag = payday2_control;
            profile_payday2.Source = new BitmapImage(new Uri(@"Resources/pd2_64x64.png", UriKind.Relative));
            profile_payday2.ToolTip = "Payday 2 Settings";
            profile_payday2.Margin = new Thickness(0, 5, 0, 0);
            profile_payday2.MouseDown += ProfileImage_MouseDown;
            this.profiles_stack.Children.Add(profile_payday2);

            Image profile_thedivision = new Image();
            profile_thedivision.Tag = division_control;
            profile_thedivision.Source = new BitmapImage(new Uri(@"Resources/division_64x64.png", UriKind.Relative));
            profile_thedivision.ToolTip = "The Division Settings";
            profile_thedivision.Margin = new Thickness(0, 5, 0, 0);
            profile_thedivision.MouseDown += ProfileImage_MouseDown;
            this.profiles_stack.Children.Add(profile_thedivision);

            Image profile_leagueoflegends = new Image();
            profile_leagueoflegends.Tag = lol_control;
            profile_leagueoflegends.Source = new BitmapImage(new Uri(@"Resources/leagueoflegends_48x48.png", UriKind.Relative));
            profile_leagueoflegends.ToolTip = "League of Legends Settings";
            profile_leagueoflegends.Margin = new Thickness(0, 5, 0, 0);
            profile_leagueoflegends.MouseDown += ProfileImage_MouseDown;
            this.profiles_stack.Children.Add(profile_leagueoflegends);

            Image profile_hotline = new Image();
            profile_hotline.Tag = hotline_control;
            profile_hotline.Source = new BitmapImage(new Uri(@"Resources/hotline_32x32.png", UriKind.Relative));
            profile_hotline.ToolTip = "Hotline Miami Settings";
            profile_hotline.Margin = new Thickness(0, 5, 0, 0);
            profile_hotline.MouseDown += ProfileImage_MouseDown;
            this.profiles_stack.Children.Add(profile_hotline);

            Image profile_talosprinciple = new Image();
            profile_talosprinciple.Tag = talosprinciple_control;
            profile_talosprinciple.Source = new BitmapImage(new Uri(@"Resources/talosprinciple_64x64.png", UriKind.Relative));
            profile_talosprinciple.ToolTip = "Hotline Miami Settings";
            profile_talosprinciple.Margin = new Thickness(0, 5, 0, 0);
            profile_talosprinciple.MouseDown += ProfileImage_MouseDown;
            this.profiles_stack.Children.Add(profile_talosprinciple);

            //Populate with added profiles
            foreach (KeyValuePair<string, Profiles.Generic_Application.GenericApplicationSettings> kvp in Global.Configuration.additional_profiles)
            {
                Image profile = new Image();
                profile.Tag = new Profiles.Generic_Application.Control_GenericApplication(kvp.Key);

                if(System.IO.File.Exists(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "./Profiles/" + kvp.Key.GetHashCode().ToString("X") + ".png"))
                {
                    var memStream = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"./Profiles/" + kvp.Key.GetHashCode().ToString("X") + ".png"));
                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.StreamSource = memStream;
                    b.EndInit();

                    profile.Source = b;

                }
                else
                    profile.Source = new BitmapImage(new Uri(@"Resources/addprofile_icon.png", UriKind.Relative));

                profile.ToolTip = kvp.Value.ApplicationName + " Settings";
                profile.MouseDown += ProfileImage_MouseDown;

                Image profile_remove = new Image();
                profile_remove.Source = new BitmapImage(new Uri(@"Resources/removeprofile_icon.png", UriKind.Relative));
                profile_remove.ToolTip = "Remove " + kvp.Value.ApplicationName + " Profile";
                profile_remove.HorizontalAlignment = HorizontalAlignment.Right;
                profile_remove.VerticalAlignment = VerticalAlignment.Bottom;
                profile_remove.Height = 16;
                profile_remove.Width = 16;
                profile_remove.Visibility = Visibility.Hidden;
                profile_remove.MouseDown += RemoveProfile_MouseDown;
                profile_remove.Tag = kvp.Key;

                Grid profile_grid = new Grid();
                profile_grid.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                profile_grid.Margin = new Thickness(0, 5, 0, 0);
                profile_grid.Tag = profile_remove;
                profile_grid.MouseEnter += Profile_grid_MouseEnter;
                profile_grid.MouseLeave += Profile_grid_MouseLeave;
                profile_grid.Children.Add(profile);
                profile_grid.Children.Add(profile_remove);


                this.profiles_stack.Children.Add(profile_grid);
            }

            //Add new profiles button
            Image profile_add = new Image();
            profile_add.Source = new BitmapImage(new Uri(@"Resources/addprofile_icon.png", UriKind.Relative));
            profile_add.ToolTip = "Add a new Lighting Profile";
            profile_add.Margin = new Thickness(0, 5, 0, 0);
            profile_add.MouseDown += AddProfile_MouseDown;
            this.profiles_stack.Children.Add(profile_add);
        }

        private void Profile_grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender != null && sender is Grid && (sender as Grid).Tag != null && (sender as Grid).Tag is Image)
            {
                ((sender as Grid).Tag as Image).Visibility = Visibility.Hidden;
            }
        }

        private void Profile_grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender != null && sender is Grid && (sender as Grid).Tag != null && (sender as Grid).Tag is Image)
            {
                ((sender as Grid).Tag as Image).Visibility = Visibility.Visible;
            }
        }

        private void ProfileImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(sender != null && sender is Image && (sender as Image).Tag != null && (sender as Image).Tag is UserControl)
            {
                UserControl tagged_control = (sender as Image).Tag as UserControl;

                this.content_grid.Children.Clear();
                this.content_grid.Children.Add(tagged_control);

                if (tagged_control is Profiles.Dota_2.Control_Dota2)
                    current_color = dota2_color_scheme;
                else if (tagged_control is Profiles.CSGO.Control_CSGO)
                    current_color = csgo_color_scheme;
                else if (tagged_control is Profiles.GTA5.Control_GTA5)
                    current_color = gta5_color_scheme;
                else if (tagged_control is Profiles.RocketLeague.Control_RocketLeague)
                    current_color = rocketleague_color_scheme;
                else
                    current_color = desktop_color_scheme;

                var bitmap = (BitmapSource)(sender as Image).Source;
                var color = GetAverageColor(bitmap);

                current_color = new EffectColor(color);
                current_color *= 0.85f;

                transitionamount = 0.0f;

                UpdateProfileStackBackground(sender as FrameworkElement);
            }
        }

        private void RemoveProfile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && sender is Image && (sender as Image).Tag != null && (sender as Image).Tag is string)
            {
                string name = (sender as Image).Tag as string;

                if (Global.Configuration.additional_profiles.ContainsKey(name))
                {
                    if (System.Windows.MessageBox.Show("Are you sure you want to delete profile for " + Global.Configuration.additional_profiles[name].ApplicationName + "?", "Remove Profile", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        Global.Configuration.additional_profiles.Remove(name);
                        ConfigManager.Save(Global.Configuration);
                        GenerateProfileStack();
                    }
                }
            }
        }

        private void AddProfile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog exe_filedlg = new Microsoft.Win32.OpenFileDialog();

            exe_filedlg.DefaultExt = ".exe";
            exe_filedlg.Filter = "Executable Files (*.exe)|*.exe;";

            Nullable<bool> result = exe_filedlg.ShowDialog();

            if (result.HasValue && result == true)
            {
                string filename = System.IO.Path.GetFileName(exe_filedlg.FileName.ToLowerInvariant());

                if(Global.Configuration.additional_profiles.ContainsKey(filename))
                {
                    System.Windows.MessageBox.Show("Profile for this application already exists.\r\nSwitching to the profile.");
                }
                else
                {
                    System.Drawing.Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(exe_filedlg.FileName.ToLowerInvariant());

                    if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "./Profiles/"))
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "./Profiles/");

                    using (var icon_asbitmap = ico.ToBitmap())
                    {
                        icon_asbitmap.Save(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"./Profiles/" + filename.GetHashCode().ToString("X") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                    ico.Dispose();

                    Global.Configuration.additional_profiles.Add(filename, new Profiles.Generic_Application.GenericApplicationSettings(filename));
                    ConfigManager.Save(Global.Configuration);
                    GenerateProfileStack();
                }

                this.content_grid.Children.Clear();
                this.content_grid.Children.Add(new Profiles.Generic_Application.Control_GenericApplication(filename));

                current_color = desktop_color_scheme;
                transitionamount = 0.0f;
            }
        }

        private void DesktopControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.content_grid.Children.Clear();
            this.content_grid.Children.Add(settings_control);

            current_color = desktop_color_scheme;
            transitionamount = 0.0f;

            UpdateProfileStackBackground(sender as FrameworkElement);
        }

        private void UpdateProfileStackBackground(FrameworkElement item)
        {
            selected_item = item;

            if(selected_item != null)
            {
                DrawingBrush mask = new DrawingBrush();
                GeometryDrawing visible_region =
                    new GeometryDrawing(
                        new SolidColorBrush(Color.FromArgb(64, 0, 0, 0)),
                        null,
                        new RectangleGeometry(new Rect(0, 0, profiles_background.ActualWidth, profiles_background.ActualHeight)));

                DrawingGroup drawingGroup = new DrawingGroup();
                drawingGroup.Children.Add(visible_region);

                Point relativePoint = selected_item.TransformToAncestor(profiles_background)
                              .Transform(new Point(0, 0));

                double x = 0.0D;
                double y = relativePoint.Y - 2.0D;
                double width = profiles_background.ActualWidth;
                double height = selected_item.ActualHeight + 4.0D;

                Debug.WriteLine("x=" + x + " y=" + y + " widht=" + width + " height=" + height);

                
                if (item.Parent != null && item.Parent.Equals(profiles_stack))
                {
                    Point relativePointWithinStack = profiles_stack.TransformToAncestor(profiles_background)
                              .Transform(new Point(0, 0));

                    if (y < relativePointWithinStack.Y)
                    {
                        height -= relativePointWithinStack.Y - y;
                        y = 0;
                    }

                }
                else
                {
                    x = 0.0D;
                    y = relativePoint.Y - 2.0D;
                    width = profiles_background.ActualWidth;
                    height = selected_item.ActualHeight + 4.0D;
                }

                Debug.WriteLine("x=" + x + " y=" + y + " widht=" + width + " height=" + height);

                if (height > 0 && width > 0)
                {

                    GeometryDrawing transparent_region =
                        new GeometryDrawing(
                            new SolidColorBrush((Color)current_color),
                            null,
                            new RectangleGeometry(new Rect(x, y, width, height)));

                    drawingGroup.Children.Add(transparent_region);
                }

                mask.Drawing = drawingGroup;

                profiles_background.Background = mask;
            }
        }

        public Color GetAverageColor(BitmapSource bitmap)
        {
            var format = bitmap.Format;

            if (format != PixelFormats.Bgr24 &&
                format != PixelFormats.Bgr32 &&
                format != PixelFormats.Bgra32 &&
                format != PixelFormats.Pbgra32)
            {
                throw new InvalidOperationException("BitmapSource must have Bgr24, Bgr32, Bgra32 or Pbgra32 format");
            }

            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;
            var numPixels = width * height;
            var bytesPerPixel = format.BitsPerPixel / 8;
            var pixelBuffer = new byte[numPixels * bytesPerPixel];

            bitmap.CopyPixels(pixelBuffer, width * bytesPerPixel, 0);

            long blue = 0;
            long green = 0;
            long red = 0;

            for (int i = 0; i < pixelBuffer.Length; i += bytesPerPixel)
            {
                blue += pixelBuffer[i];
                green += pixelBuffer[i + 1];
                red += pixelBuffer[i + 2];
            }

            return Color.FromRgb((byte)(red / numPixels), (byte)(green / numPixels), (byte)(blue / numPixels));
        }

        private void trayicon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.Show();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UpdateProfileStackBackground(selected_item);
        }
    }
}