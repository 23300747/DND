using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Proyecto_Dnd
{
    public partial class Form12 : Form
    {
        private int jugadorId;
        private ListView listaLogros;
        private Label lblProgreso;
        private Panel panelDetalle;

        public Form12(int idJugador)
        {
            InitializeComponent();
            jugadorId = idJugador;
            ConfigurarUI();
            CargarLogros();
        }
        private void Form12_Load(object sender, EventArgs e)
        {
        }
        private void ConfigurarUI()
        {
            this.Text = "Logros del Aventurero";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(40, 30, 20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblTitulo = new Label
            {
                Text = "LIBRO DE LOGROS",
                Font = new Font("Papyrus", 20, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = false,
                Size = new Size(860, 50),
                Location = new Point(20, 10),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblProgreso = new Label
            {
                Font = new Font("Papyrus", 11),
                ForeColor = Color.White,
                Location = new Point(20, 70),
                AutoSize = true
            };

            listaLogros = new ListView
            {
                Location = new Point(20, 110),
                Size = new Size(550, 450),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(240, 235, 220),
                Font = new Font("Segoe UI", 10)
            };
            listaLogros.Columns.Add("Logro", 300);
            listaLogros.Columns.Add("Rareza", 120);
            listaLogros.Columns.Add("Estado", 100);
            listaLogros.SelectedIndexChanged += ListaLogros_SelectedIndexChanged;

            panelDetalle = new Panel
            {
                Location = new Point(590, 110),
                Size = new Size(290, 450),
                BackColor = Color.FromArgb(60, 45, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblDetalleTitle = new Label
            {
                Text = "DETALLES",
                Font = new Font("Papyrus", 12, FontStyle.Bold),
                ForeColor = Color.Gold,
                Location = new Point(10, 10),
                AutoSize = true
            };

            Label lblDescripcion = new Label
            {
                Name = "lblDescripcion",
                Text = "Selecciona un logro para ver detalles",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.White,
                Location = new Point(10, 50),
                Size = new Size(270, 380),
                AutoSize = false
            };

            panelDetalle.Controls.Add(lblDetalleTitle);
            panelDetalle.Controls.Add(lblDescripcion);

            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new Point(360, 575),
                Size = new Size(180, 40),
                Font = new Font("Papyrus", 12, FontStyle.Bold),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCerrar.FlatAppearance.BorderColor = Color.Gold;
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblProgreso);
            this.Controls.Add(listaLogros);
            this.Controls.Add(panelDetalle);
            this.Controls.Add(btnCerrar);
        }

        private void CargarLogros()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("CALL ObtenerLogrosJugador(@id)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", jugadorId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            int total = 0;
                            int desbloqueados = 0;

                            while (reader.Read())
                            {
                                total++;
                                string nombre = reader.GetString("Nombre");
                                string descripcion = reader.GetString("Descripcion");
                                string rareza = reader.GetString("Rareza");
                                bool desbloqueado = reader.GetInt32("Desbloqueado") == 1;
                                string fecha = reader.IsDBNull(reader.GetOrdinal("FechaDesbloqueo"))
                                    ? ""
                                    : reader.GetDateTime("FechaDesbloqueo").ToString("dd/MM/yyyy");

                                if (desbloqueado) desbloqueados++;

                                ListViewItem item = new ListViewItem(nombre);
                                item.SubItems.Add(rareza);
                                item.SubItems.Add(desbloqueado ? "Desbloqueado" : "Bloqueado");

                                item.Tag = new
                                {
                                    Descripcion = descripcion,
                                    Rareza = rareza,
                                    Desbloqueado = desbloqueado,
                                    Fecha = fecha
                                };

                                if (desbloqueado)
                                {
                                    item.BackColor = Color.LightGreen;
                                    item.ForeColor = Color.DarkGreen;
                                }
                                else
                                {
                                    item.ForeColor = Color.Gray;
                                }

                                listaLogros.Items.Add(item);
                            }

                            lblProgreso.Text = $"Progreso: {desbloqueados}/{total} logros desbloqueados";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar logros: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListaLogros_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listaLogros.SelectedItems.Count > 0)
            {
                var item = listaLogros.SelectedItems[0];
                dynamic datos = item.Tag;

                Label lblDesc = panelDetalle.Controls["lblDescripcion"] as Label;

                string detalle = $"{item.SubItems[0].Text}\n\n";
                detalle += $"Rareza: {datos.Rareza}\n\n";
                detalle += $"Descripción:\n{datos.Descripcion}\n\n";
                detalle += $"Estado: {(datos.Desbloqueado ? "DESBLOQUEADO" : "Bloqueado")}\n\n";

                if (datos.Desbloqueado && !string.IsNullOrEmpty(datos.Fecha))
                {
                    detalle += $"Fecha: {datos.Fecha}";
                }

                lblDesc.Text = detalle;
            }
        }
    }
}