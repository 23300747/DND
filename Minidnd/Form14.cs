using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

namespace Proyecto_Dnd
{
    public partial class Form14 : Form
    {
        private Panel panelCreditos;
        private System.Windows.Forms.Timer timerScroll;
        private int posicionY = 600; // arranca desde abajo
        private int offsetY = 0;     // acumulador dinámico
        SoundPlayer musica;

        public Form14()
        {
            InitializeComponent();
            ConfigurarUI();
            IniciarAnimacion();
            ReproducirMusica();
        }
        private void Form14_Load(object sender, EventArgs e)
        {
        }
        private void ConfigurarUI()
        {
            this.Text = "Créditos - Dragones de la Isla de las Tempestades";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            panelCreditos = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(800, 600),
                BackColor = Color.Black
            };

            this.Controls.Add(panelCreditos);

            Button btnCerrar = new Button
            {
                Text = "Volver",
                Location = new Point(330, 540),
                Size = new Size(140, 35),
                Font = new Font("Papyrus", 10, FontStyle.Bold),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCerrar.FlatAppearance.BorderColor = Color.Gold;
            btnCerrar.Click += (s, e) => this.Close();
            btnCerrar.BringToFront();
            this.Controls.Add(btnCerrar);
        }

        private void IniciarAnimacion()
        {
            // ahora no pasamos coordenadas fijas, solo texto
            AgregarTextoCredito("DRAGONES DE LA ISLA DE LAS TEMPESTADES", Color.Gold, true);
            AgregarTextoCredito("Un proyecto de Dungeons & Dragons", Color.LightGray);

            AgregarTextoCredito("DESARROLLO", Color.Gold, true);
            AgregarTextoCredito("Alonso, Gilberto, Gael", Color.White);

            AgregarTextoCredito("BASE DE DATOS", Color.Gold, true);
            AgregarTextoCredito("Alonso, Gilberto, Gael, Ivan", Color.White);

            AgregarTextoCredito("DISEÑO VISUAL", Color.Gold, true);
            AgregarTextoCredito("Alonso, Ivan", Color.White);

            AgregarTextoCredito("DOCUMENTACIÓN", Color.Gold, true);
            AgregarTextoCredito("Axel, Zabdiel", Color.White);

            AgregarTextoCredito("TECNOLOGÍAS", Color.Gold, true);
            AgregarTextoCredito("C# 14 - .NET Framework 8", Color.White);
            AgregarTextoCredito("Windows Forms", Color.White);
            AgregarTextoCredito("MariaDB 12.1.1", Color.White);

            AgregarTextoCredito("BASADO EN", Color.Gold, true);
            AgregarTextoCredito("Dungeons & Dragons 5ta Edición", Color.White);
            AgregarTextoCredito("Los Dragones de la Isla de las Tempestades", Color.White);

            AgregarTextoCredito("RECURSOS", Color.Gold, true);
            AgregarTextoCredito("Imágenes y sprites tanto hechos a mano como de dominio público", Color.White);
            AgregarTextoCredito("Un sistema de reglas parecido a DnD", Color.White);

            AgregarTextoCredito("2025 - Proyecto Integrador", Color.LightGray);
            AgregarTextoCredito("Base de Datos y Métodos y Metodologías de desarrollo de software", Color.LightGray);

            AgregarTextoCredito("¡Disfrute la experiencia visual!", Color.Gold, true);

            timerScroll = new System.Windows.Forms.Timer
            {
                Interval = 30
            };
            timerScroll.Tick += TimerScroll_Tick;
            timerScroll.Start();
        }

        private void AgregarTextoCredito(string texto, Color color, bool esTitulo = false)
        {
            Label lbl = new Label
            {
                Text = texto,
                Location = new Point(0, offsetY + posicionY),
                Size = new Size(800, esTitulo ? 40 : 30),
                Font = esTitulo ? new Font("Papyrus", 16, FontStyle.Bold) : new Font("Papyrus", 12),
                ForeColor = color,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };

            panelCreditos.Controls.Add(lbl);
            offsetY += esTitulo ? 50 : 35; 
        }

        private void TimerScroll_Tick(object sender, EventArgs e)
        {
            foreach (Control ctrl in panelCreditos.Controls)
            {
                if (ctrl is Label)
                {
                    ctrl.Top -= 1;
                }
            }

            if (panelCreditos.Controls.Count > 0)
            {
                Label ultimaLabel = panelCreditos.Controls[panelCreditos.Controls.Count - 1] as Label;
                if (ultimaLabel != null && ultimaLabel.Top + ultimaLabel.Height < 0)
                {
                    timerScroll.Stop();
                }
            }
        }

        private void ReproducirMusica()
        {
            musica = new SoundPlayer("final.wav");
            musica.PlayLooping();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (timerScroll != null)
            {
                timerScroll.Stop();
                timerScroll.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}