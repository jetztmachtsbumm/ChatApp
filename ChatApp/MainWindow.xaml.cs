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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Collection<UserModel> users { get; set; }

        Server server;

        public MainWindow()
        {
            InitializeComponent();
            users = new ObservableCollection<UserModel>();
            server = new Server();
            server.connectedEvent += UserConnected;
            server.msgReceivedEvent += MSG_Received;
            server.userDisconnectEvent += UserDisconnectEvent;
        }

        private void UserDisconnectEvent()
        {
            string uid = server.GetPacketReader().ReadMessage();
            UserModel user = users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => { 
                users.Remove(user);
                UserView.Items.Remove(user);
            });
        }

        private void MSG_Received()
        {
            string msg = server.GetPacketReader().ReadMessage();
            Application.Current.Dispatcher.Invoke(() => MsgView.Items.Add(msg));
        }

        private void UserConnected()
        {
            UserModel user = new UserModel()
            {
                Username = server.GetPacketReader().ReadMessage(),
                UID = server.GetPacketReader().ReadMessage(),
            };

            if(!users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => users.Add(user));
            }

            Application.Current.Dispatcher.Invoke(() => UserView.Items.Clear());
            foreach (UserModel usr in users)
            {
                Application.Current.Dispatcher.Invoke(() => UserView.Items.Add(usr.Username));
            }
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            server.ConnectToServer(NicknameTextBox.Text);
        }

        private void NicknameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            ConnectBtn.IsEnabled = !string.IsNullOrEmpty(NicknameTextBox.Text);
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            server.SendMessage(MessageTextBox.Text);
        }

    }
}
