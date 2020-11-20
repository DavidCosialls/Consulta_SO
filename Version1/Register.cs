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

namespace Version1
{

    public partial class Register : Form
    {
        Socket server;
        public Register()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Register_Load(object sender, EventArgs e)
        {

        }

        private void SaveButton_Click_1(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor
            //al que deseamos conectarnos

            if ((Comprovation.Text != "") && (PasswordBox.Text != "") && (UsernameBox.Text != ""))
            {
                if (PasswordBox.Text == Comprovation.Text)
                {
                    //"147.83.117.22" Shiva
                    //192.168.56.102 Maquina virtual
                    IPAddress direc = IPAddress.Parse("192.168.56.101");//DireccionIP de la Maquina Virtual
                    IPEndPoint ipep = new IPEndPoint(direc, 9012); //Le pasamos el acceso y el puerto que asignamos en el codigo del servidor

                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Parámetros estándard
                    try
                    {
                        server.Connect(ipep); //Intentamos conectar el socket
                        this.BackColor = Color.Green;
                        //MessageBox.Show("conectado");
                    }
                    catch (SocketException ex)
                    {
                        //Si hay excepción imprimimos error y salimos del programa con return
                        MessageBox.Show("No se ha podido conectar con el servidor");
                        return;
                    }
                    string mensaje = "5/" + UsernameBox.Text + "/" + PasswordBox.Text;
                    //Envimos al servidor el nombre tecleado
                    //Cogemos el string creado y lo convertimos en un vector de Bytes
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor
                    //Recibimos un vector de bytes y lo convertimos a string
                    byte[] msg2 = new byte[30];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0]; //El split sirve para quedarme solo con el string que quiero
                                                                             //lo demás se considera basura
                    if (mensaje == "REGISTRADO")
                    {
                        MessageBox.Show("Usuario registrado");
                    }
                    else if (mensaje == "NO")
                    {
                        MessageBox.Show("Usuario ya registrado.");
                    }

                    mensaje = "0/";

                    msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    this.BackColor = Color.Gray;
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                    this.Close();
                }
                else
                    MessageBox.Show("The passwords don't match");


            }
            else
                MessageBox.Show("Introduzca un usuario y contraseña"); 
        }
        
        

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (PasswordBox.PasswordChar == Convert.ToChar("*"))
                {
                    PasswordBox.PasswordChar = Convert.ToChar("\0");
                    Comprovation.PasswordChar = Convert.ToChar("\0");

                }
            }
            else
            {
                PasswordBox.PasswordChar = Convert.ToChar("*");
                Comprovation.PasswordChar = Convert.ToChar("*");

            }
            
            
        }
    }
}
