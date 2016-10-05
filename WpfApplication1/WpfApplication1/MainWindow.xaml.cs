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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Reflection;
using System.Data.OleDb;
using System.Data;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        DispatcherTimer timer1 = new DispatcherTimer();
        DispatcherTimer timer2 = new DispatcherTimer();
        DispatcherTimer timer3 = new DispatcherTimer();
        DispatcherTimer timer4 = new DispatcherTimer();
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;
        [DllImport("user32.dll")]
        private static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        private List<string> Account = new List<string>();
        private List<string> Password = new List<string>();
        private List<RadioButton> Rb = new List<RadioButton>();
        private List<int[,]> personaj = new List<int[,]>();
        private int select_personaj = 100;
        private int select_Account;
        private string s = @"d:\Games\World of Warcraft(WowCircle)\Wow.exe";
        private string connection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Database2.accdb";

        ThicknessAnimation Anim_enter = new ThicknessAnimation();
        DoubleAnimation DobleAnim_enter = new DoubleAnimation();
        DoubleAnimation DobleAnim_enter2 = new DoubleAnimation();

        ThicknessAnimation Anim_leave = new ThicknessAnimation();
        DoubleAnimation DobleAnim_leave = new DoubleAnimation();
        DoubleAnimation DobleAnim_leave2 = new DoubleAnimation();
        private double time_anim = 0.5;

        public MainWindow()
        {
            InitializeComponent();
            using (OleDbConnection conn = new OleDbConnection(connection))
            {
                OleDbCommand cmd = new OleDbCommand("select Account_name from Account", conn);
                OleDbCommand cmd1 = new OleDbCommand("select Password from Account", conn);
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                OleDbDataReader dr = cmd.ExecuteReader();
                OleDbDataReader dr1 = cmd1.ExecuteReader();
                if (dr.HasRows && dr1.HasRows)
                {
                    while (dr.Read() && dr1.Read())
                    {
                        comboBox1.Items.Add(dr["Account_name"]);
                        Account.Add(dr["Account_name"].ToString());
                        Password.Add(dr1["Password"].ToString());
                    }
                    comboBox1.SelectedItem = comboBox1.Items[comboBox1.SelectedIndex + 1];
                }
                conn.Close();
            }

            this.Background = new ImageBrush(new BitmapImage(new Uri("ilidan.jpg", UriKind.Relative)));

            timer1.Interval = new TimeSpan(0, 0, 15);
            timer1.Tick += new EventHandler(Insert);
            timer2.Interval = new TimeSpan(0, 0, 1);
            timer2.Tick += new EventHandler(Insert2);
            timer3.Interval = new TimeSpan(0, 0, 5);
            timer3.Tick += new EventHandler(Press_to_LBM);
            timer4.Interval = new TimeSpan(0, 0, 1);
            timer4.Tick += new EventHandler(Press_to_ENTER);

            DobleAnim_enter2.To = 220;
            DobleAnim_enter2.Duration = TimeSpan.FromSeconds(time_anim);
            DobleAnim_enter.Duration = TimeSpan.FromSeconds(time_anim);
            DobleAnim_enter.To = 98;
            Anim_enter.Duration = TimeSpan.FromSeconds(time_anim);
            Anim_enter.To = new Thickness(image1.Margin.Left - 19, image1.Margin.Top - 17, image1.Margin.Right, image1.Margin.Bottom);

            DobleAnim_leave2.To = 182;
            DobleAnim_leave2.Duration = TimeSpan.FromSeconds(time_anim);
            DobleAnim_leave.Duration = TimeSpan.FromSeconds(time_anim);
            DobleAnim_leave.To = 93;
            Anim_leave.Duration = TimeSpan.FromSeconds(time_anim);
            Anim_leave.To = new Thickness(image1.Margin.Left, image1.Margin.Top, image1.Margin.Right, image1.Margin.Bottom);

            ImageSourceConverter converter = new ImageSourceConverter();
            string path = string.Format(@"{0}\{1}", (System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)), "ilidan.gif");
            ImageSource imageSource = (ImageSource)converter.ConvertFromString(path);
            image1.Source = imageSource;
            this.Icon = new BitmapImage(new Uri("12834133.ico", UriKind.RelativeOrAbsolute));
        }

        private void Insert(object sender, EventArgs e)
        {
            Clipboard.SetText(Account[select_Account]);
            Ctrl_V_Emul();
            Tab_Emul();
            timer1.Stop();
            timer2.Start();
        }

        private void Insert2(object sender, EventArgs e)
        {
            Clipboard.SetText(Password[select_Account]); // Выбор пароля в зависимости от выбранного аккаунта
            Ctrl_V_Emul();
            Enter_Emul();
            image1.IsEnabled = true;
            timer2.Stop();
            timer3.Start();
        }

        private void Press_to_LBM(object sender, EventArgs e)
        {
            if (select_personaj == 100)
            {
                timer3.Stop();
            }
            else
            {
                int[,] buf = personaj[select_personaj]; // Выбираем из списка координат, координаты выбраного персонажа и заносим в буфер чтобы передать в функцию
                SetCursorPos(buf[0, 0], buf[0, 1]);
                LBM_Emul();
                timer4.Start();
                timer3.Stop();
            }
        }

        private void Press_to_ENTER(object sender, EventArgs e)
        {
            Enter_Emul();
            SetCursorPos((int)SystemParameters.PrimaryScreenWidth / 2, (int)SystemParameters.PrimaryScreenHeight / 2);
            timer4.Stop();
        }

        private void Ctrl_V_Emul()
        {
            keybd_event(0x11, 0, 0, 0);
            keybd_event((byte)'V', 0, 0, 0);
            keybd_event((byte)'V', 0, 0x2, 0);
            keybd_event(0x11, 0, 0x2, 0);
        }

        private void Tab_Emul()
        {
            keybd_event(0x09, 0, 0, 0);
            keybd_event(0x09, 0, 0x2, 0);
        }

        private void Enter_Emul()
        {
            keybd_event(0x0D, 0, 0, 0);
            keybd_event(0x0D, 0, 0x2, 0);
        }

        private void LBM_Emul()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, new IntPtr());
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, new IntPtr());
        }

        private void radioButton_Select(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Rb.Count; i++)
            {
                if (Rb[i].IsChecked == true)
                    select_personaj = i;
            }
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            select_personaj = 100; // Для того чтобы, если не был выбран персонаж, не происходил вход в игру, каким либо персонажем
            groupBox1.Visibility = System.Windows.Visibility.Visible;
            groupBox1.Margin = new Thickness(42, 74, 0, 0);
            RadioButtonChecked();
            for (int i = 0; i < Account.Count; i++)
            {
                if (Account[i] == comboBox1.SelectedItem.ToString())
                {
                    select_Account = i; // Выбор аккаунта для входа
                    CreateGroupBoxNickName(); // Формирование groupbox'а
                }
            }
        }

        private void Mouse_Enter(object sender, MouseEventArgs e)
        {
            image1.BeginAnimation(Image.MarginProperty, Anim_enter);
            image1.BeginAnimation(Image.WidthProperty, DobleAnim_enter2);
            image1.BeginAnimation(Image.HeightProperty, DobleAnim_enter);
        }

        private void Mouse_Leave(object sender, MouseEventArgs e)
        {
            image1.BeginAnimation(Image.MarginProperty, Anim_leave);
            image1.BeginAnimation(Image.WidthProperty, DobleAnim_leave2);
            image1.BeginAnimation(Image.HeightProperty, DobleAnim_leave);
        }

        private void Mouse_LeftBt_Up(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = s;
                proc.Start();
                timer1.Start();
                image1.IsEnabled = false;
                image1.BeginAnimation(Image.MarginProperty, Anim_leave);
                image1.BeginAnimation(Image.WidthProperty, DobleAnim_leave2);
                image1.BeginAnimation(Image.HeightProperty, DobleAnim_leave);
            }
            catch
            {
                MessageBox.Show("Выберите исполнимый файл World of Warcraft, Wow.exe!", "Файл не выбран", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RadioButtonChecked()
        {
            for (int i = 0; i < Rb.Count; i++)
            {
                Rb[i].IsChecked = false;
            }
        }

        private void CreateGroupBoxNickName()
        {
            for (int i = 0; i < Rb.Count; i++)
            {
                Rb[i].Visibility = System.Windows.Visibility.Hidden;
            }
            string query = "select Nick from " + comboBox1.Items[comboBox1.SelectedIndex].ToString();
            using (OleDbConnection conn = new OleDbConnection(connection))
            {
                OleDbCommand cmd = new OleDbCommand(query, conn);
                conn.Open();
                OleDbDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    int i = 0;
                    while (dr.Read())
                    {
                        RadioButton radiob1 = new RadioButton();
                        radiob1.Name = "radioButton" + i.ToString();
                        //
                        // Избавления добавления повторяющихся radiobutton'в, добавление в случае, если в предыдущем аккаунте было меньше персонажей
                        //
                        if (i >= Rb.Count)
                        {
                            //
                            // Создаем переменную buf для того чтобы вносить данные о положении (в пикселях) персонажа в окне выбора персонажей далее вносим данные в список координат
                            //
                            int[,] buf = { { ((int)SystemParameters.PrimaryScreenWidth * 87) / 100, (int)(SystemParameters.PrimaryScreenHeight * (13.88 + i * 7.13)) / 100 } };
                            //------------------|
                            personaj.Add(buf);//|
                            //------------------|
                            Rb.Add(radiob1);
                            grid_in_groupBox1.Children.Add(Rb[i]);
                            Rb[i].Margin = new Thickness(10, 10 + i * 21, 0, 0);
                            Rb[i].Checked += radioButton_Select;
                        }                               // ^
                        Rb[i].Content = dr["Nick"];     // |
                        //                                 | 
                        // Настройка отображения radiobutton'ов и высоту groupbox'а их позиционирования в зависимости от кол-ва персонажей на аккаунте
                        //
                        Rb[i].Visibility = System.Windows.Visibility.Visible;
                        groupBox1.Height = 53 + i * 19;
                        i++;
                    }
                }
                conn.Close();
            }
        }
    }
}