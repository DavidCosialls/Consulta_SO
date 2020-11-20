using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Version1
{
    public partial class Consultas : Form
    {
        Login l = new Login();
        Socket server;
        bool disconecting = false; 
        string usuario;
        int id;
        bool conectado = false;
        int contador_servicios;
        string socket;
        Thread atender;


        public Consultas()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false; 
            if (this.BackColor != Color.Green)
                SendButton.Enabled = false; 
        }
        public void SetSocket (string t)
        {
            this.socket = t; 
        }
        private void Consultas_Load(object sender, EventArgs e)
        {

            
            this.CenterToScreen();

            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor
            //al que deseamos conectarnos
            conectado = true;

            UsuarioLabel.Text = "User: "+ this.usuario;
            IdLabel.Text = "ID: " + Convert.ToString(this.id);
            //"147.83.117.22" Shiva
            //192.168.56.102 Maquina virtual
            IPAddress direc = IPAddress.Parse("192.168.56.101");//DireccionIP de la Maquina Virtual
            IPEndPoint ipep = new IPEndPoint(direc, 9012); //Le pasamos el acceso y el puerto que asignamos en el codigo del servidor

            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Parámetros estándard
            try
            {
                server.Connect(ipep); //Intentamos conectar el socket
                this.BackColor = Color.Green;
                SendButton.Enabled = true;
                //MessageBox.Show("conectado");
            }
            catch (SocketException ex)
            {
                //Si hay excepción imprimimos error y salimos del programa con return
                MessageBox.Show("No se ha podido conectar con el servidor");
                return;
            }
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();

            //NOS VAMOS A PONER COMO ONLINE

            int calculo = Convert.ToInt32(socket);
            calculo++;
            socket = Convert.ToString(calculo);
            enviar_server("7/" + socket + "/" + this.id);
            conectado = true;
            

            //ponemos en marcha el thread que atenderá los mensajes del servidor
            


        }
        public void SetUsername (string user)  //RECIBIMOS EL USUARIO CONECTADO
        { 
            this.usuario = user;
        }
        public void SetId(string identifier) // RECIBIMOS EL ID DEL USUARIO CONECTADO
        {
            int id_1 = Convert.ToInt32(identifier);
            this.id = id_1;

        }
        public void AtenderServidor()
        {
            while (true)
            {
                //Recibimos un vector de bytes y lo convertimos a string
                byte[] msg2 = new byte[512];
                server.Receive(msg2);
                if (msg2[0] != 0)
                {
                    string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                    int codigo = Convert.ToInt32(trozos[0]);
                    string mensaje;

                    switch (codigo)
                    {
                        case 1: //Vicotrias jugador concreto
                            mensaje = trozos[1].Split('\0')[0];
                            MessageBox.Show("El numero de partidas ganadas por el usuario son: " + mensaje);
                            break;
                        case 2: //Dia con mas partidas jugadas

                            mensaje = trozos[1].Split('\0')[0];
                            MessageBox.Show("El dia que se han jugado mas partidas es: " + mensaje);
                            break;
                        case 3:
                            // RESETEAMOS LA GRID PARA LOS DATOS
                            string index2 = trozos[1].Split('/')[0];
                            dataGridView1.Rows.Clear();
                            dataGridView1.Refresh();

                            //CONSULTAMOS AL SERVIDOR SOBRE EL NUMERO DE JUGADORES
                            //PREGUNTANDO EL ID MÁS GRANDE

                            int i = 2;
                            int total2 = Convert.ToInt32(index2);

                            while (i <= total2 + 1)
                            {
                                string istr = Convert.ToString(i - 1);
                                string username = trozos[i].Split('/')[0];

                                if (i != 2)
                                {
                                    //IMPRIMIMOS LOS RESULTADOS EN LA GRID DEL FORMS
                                    int n = dataGridView1.Rows.Add();
                                    dataGridView1.Rows[n].Cells[0].Value = username;
                                    dataGridView1.Rows[n].Cells[1].Value = istr;

                                }

                                i++;
                            }
                            dataGridView1.Refresh();
                            break;
                        case 4:
                            string index = trozos[1].Split('/')[0]; //4/IDMAX/Player1/Player2
                            dataGridView2.Rows.Clear();
                            dataGridView2.Refresh();

                            int total = Convert.ToInt32(index);
                            i = 2;
                            while (i < total + 2)
                            {
                                string username = trozos[i].Split('/')[0];
                                if ((username != "NO") && (i != 2)) //SI NOS DEVUELVE NO ESE USUARIO NO ESTA CONECTADO
                                {
                                    int n = dataGridView2.Rows.Add();
                                    dataGridView2.Rows[n].Cells[0].Value = username;
                                    dataGridView2.Refresh();
                                    
                                }
                                i++;
                            }
                            dataGridView2.Refresh();

                            break;

                    }
                }
            }
        }


        private string consulta_server(string mensaje)  //UNA FUNCION PARA SIMPLIFICAR EL ENVIAR DATOS AL SERVER
        {

            //Envimos al servidor el nombre tecleado
            //Cogemos el string creado y lo convertimos en un vector de Bytes
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            //Recibimos la respuesta del servidor
            //Recibimos un vector de bytes y lo convertimos a string
            byte[] msg2 = new byte[30];
            server.Receive(msg2);
            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

            return mensaje;
        }
        private void enviar_server(string mensaje)  //UNA FUNCION PARA SIMPLIFICAR EL ENVIAR DATOS AL SERVER
        {

            //Envimos al servidor el nombre tecleado
            //Cogemos el string creado y lo convertimos en un vector de Bytes
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

           
        }


        private void ConnectButton_Click(object sender, EventArgs e)  //NOS CONECTAMOS AL SERVIDOR 
        {

        }

        private void SendButton_Click(object sender, EventArgs e)
        {

            if(GamesWonButton.Checked)
            {
                enviar_server("1/" + UsernameBox.Text);          
            }

            if (MoreGamesButton.Checked)
            {

                enviar_server("2/ALGO");

            }

            if (PlayersListButton.Checked)
            {

                // RESETEAMOS LA GRID PARA LOS DATOS

                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();

                //CONSULTAMOS AL SERVIDOR SOBRE EL NUMERO DE JUGADORES
                //PREGUNTANDO EL ID MÁS GRANDE

                enviar_server("3/IDMAX");
            }
            

        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            atender.Abort(); 
            disconecting = true; 

            UsuarioLabel.Text = "";
            UsuarioLabel.Text = "";

            // NOS QUITAMOS DEL MODO ONLINE
            
            conectado = false;
            enviar_server("8/" + Convert.ToString(id));

            //NOS DESCONECTAMOS DEL SOCKET
            enviar_server("0/Desconectado");

            //Nos desconectamos
            this.BackColor = Color.Gray;
            SendButton.Enabled = false;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            this.Close();
            l.ShowDialog();

            
            
        }

        private void Consultas_FormClosing_1(object sender, FormClosingEventArgs e)
        {

            // El formulario se está cerrando. Llamamos al evento
            // Click del control Button1.
            //Mensaje de desconexión
            if (disconecting == false)
            {
                if (MessageBox.Show("Desea salir del programa", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (conectado == true)
                    {
                        atender.Abort(); 
                        enviar_server("8/" + Convert.ToString(id));

                        enviar_server("0/Desconectado");

                        //Nos desconectamos
                        this.BackColor = Color.Gray;
                        SendButton.Enabled = false;
                        server.Shutdown(SocketShutdown.Both);
                        server.Close();
                         
                    }
                    disconecting = true; 
                    Application.Exit();
                }
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //FUNCIONES PARA QUITAR Y PONER LA BARRA DE TEXTO Y LAS TABLAS DE DATOS
        private void GamesWonButton_CheckedChanged(object sender, EventArgs e)
        {
            UsernameBox.Enabled = true;
            UsernameBox.Text = "Write here the username";




            
        }

        private void MoreGamesButton_CheckedChanged(object sender, EventArgs e)
        {
            UsernameBox.Enabled = false;

            if (MoreGamesButton.Checked)
            {
   
                enviar_server("2/ALGO");

                
            }

        }

        private void PlayersListButton_CheckedChanged(object sender, EventArgs e)
        {

            UsernameBox.Enabled = false;
            

            if (PlayersListButton.Checked)
            {
                enviar_server("3/IDMAX");
            }

        }

        

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void UsernameBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void UsernameBox_Click(object sender, EventArgs e)
        {
            UsernameBox.Text = ""; 
        }

        private void OnlineButton_Click(object sender, EventArgs e)
        {
            
        }
    }
}

