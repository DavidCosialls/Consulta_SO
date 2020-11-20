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
    public partial class Login : Form
    {
        Socket server;
        Thread atender;
        public Login()
        {
            InitializeComponent();
            this.CenterToScreen(); 
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            Register r = new Register();
            r.ShowDialog();
            this.CenterToScreen();
        }
        private void Login_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
        }
        private string consulta_server (string mensaje)
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

        private void StartButton_Click(object sender, EventArgs e)
        {
            string usuario = textBox1.Text;
            string pwd = textBox2.Text;

            if ((usuario != "") && (pwd != ""))
            {

                IPAddress direc = IPAddress.Parse("192.168.56.101");//DireccionIP de la Maquina Virtual
                IPEndPoint ipep = new IPEndPoint(direc, 9012); //Le pasamos el acceso y el puerto que asignamos en el codigo del servidor

                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Parámetros estándard
                try
                {
                    server.Connect(ipep); //Intentamos conectar el socket

                    //MessageBox.Show("conectado");
                }
                catch (SocketException ex)
                {
                    //Si hay excepción imprimimos error y salimos del programa con return
                    MessageBox.Show("No se ha podido conectar con el servidor");
                    return;
                }

                // ENVIAMOS EL USUARIO Y RECIBIMOS CONTRASEÑA

                string mensaje = "4/" + usuario;
                //Envimos al servidor el nombre tecleado
                //Cogemos el string creado y lo convertimos en un vector de Bytes
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                //Recibimos un vector de bytes y lo convertimos a string
                byte[] msg2 = new byte[30];
                server.Receive(msg2);
                string[] message = Encoding.ASCII.GetString(msg2).Split('/'); //El split sirve para quedarme solo con el string que quiero
                string contraseña = message[0];                                                         //lo demás se considera basura
                if (pwd == contraseña) // SI LA CONTRASEÑA COINCIDE AVANZAMOS
                {
                    MessageBox.Show("Log in Correcto");

                    Consultas consulta = new Consultas();  //GENERAMOS UN FORMULARIO DE CONSULTAS
                    consulta.SetUsername(usuario);     //LE LLEVAMOS EL USUARIO QUE SE ESTÁ CONECTANDO

                    // COGEMOS EL ID DEL USUARIO

                    string id = message[1];

                    consulta.SetId(id); // MANDAMOS EL ID DEL USUARIO AL FORM CONSULTAS

                    //PREGUNTAR QUE SOCKET ME CORRESPONDE

                   
                    consulta.SetSocket(message[2]);

                    //Mensaje de desconexión
                    

                   

                    mensaje = "0/";
                    msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    //Nos desconectamos

                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                    this.Hide();
                    consulta.ShowDialog();


                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos");
                    textBox2.Text = "";

                    //Mensaje de desconexión
                    mensaje = "0/";
                    msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    //Nos desconectamos

                    server.Shutdown(SocketShutdown.Both);
                    server.Close();

                }


            }
            else
                MessageBox.Show("Introduzca su usuario y contraseña");


        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit(); 
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (textBox2.PasswordChar == Convert.ToChar("*"))
                {
                    textBox2.PasswordChar = Convert.ToChar("\0");
                    

                }
            }
            else
            {
                textBox2.PasswordChar = Convert.ToChar("*");
               
            }
        }
    }
}
