using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace Proyecto_Dnd
{
    public partial class Form10 : Form
    {
        private int jugadorId;
        private PersonajeCompleto pj;
        private ListView listaItems;
        private Panel panelEquipo;
        private Label lblArmaEquipada, lblArmaduraEquipada, lblAccesorioEquipado;
        private Label lblEstadisticas, lblOro;
        private Button btnEquipar, btnUsar, btnDesequipar, btnCerrar;

        public Form10(int idJugador)
        {
            jugadorId = idJugador;
            InitializeComponent();
            ConfigurarUI();
            CargarPersonajeCompleto();
            CargarDatosUI();
        }

        public Form10(PersonajeCompleto personaje)
        {
            jugadorId = Form9.JugadorIdActual;

            if (jugadorId <= 0 && personaje != null && !string.IsNullOrEmpty(personaje.Nombre))
            {
                jugadorId = ObtenerIdPorNombre(personaje.Nombre);
            }

            InitializeComponent();
            ConfigurarUI();
            CargarPersonajeCompleto();
            CargarDatosUI();
        }

        private int ObtenerIdPorNombre(string nombrePersonaje)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();
                    using (MySqlCommand cmd = new MySqlCommand("ObtenerIdPorNombre", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("pNombre", nombrePersonaje);

                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private void CargarPersonajeCompleto()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("CALL CargarPersonajeCompleto(@id)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", jugadorId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int dadosGolpe = reader.GetInt32("DadosGolpe");
                                int nivel = reader.GetInt32("ID_Nivel");
                                int constitucion = reader.GetInt32("Constitucion");

                                pj = new PersonajeCompleto
                                {
                                    Nombre = reader.GetString("Nombre"),
                                    Clase = reader.GetString("NombreClase"),
                                    Nivel = nivel,
                                    Fuerza = reader.GetInt32("Fuerza"),
                                    Destreza = reader.GetInt32("Destreza"),
                                    Constitucion = constitucion,
                                    Inteligencia = reader.GetInt32("Inteligencia"),
                                    Sabiduria = reader.GetInt32("Sabiduria"),
                                    Carisma = reader.GetInt32("Carisma"),
                                    VidaActual = reader.GetInt32("HP"),
                                    VidaMax = dadosGolpe + (constitucion * nivel),
                                    Oro = reader.GetInt32("Oro"),
                                    Inventario = new List<Item>(),
                                    EquipoActual = new Equipo()
                                };
                            }

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    string tipo = reader.GetString("Tipo");
                                    string categoria = reader.IsDBNull(reader.GetOrdinal("Categoria")) ? "" : reader.GetString("Categoria");

                                    Item item = new Item
                                    {
                                        IdObjeto = reader.GetInt32("ID_Objeto"),
                                        Nombre = reader.GetString("Nombre"),
                                        Tipo = tipo,
                                        Categoria = categoria,
                                        Descripcion = $"Efecto: {reader.GetInt32("Efecto")}",
                                        BonVida = reader.GetInt32("Efecto"),
                                        Cantidad = reader.GetInt32("Cantidad"),
                                        Equipado = reader.GetInt32("Equipado") == 1
                                    };

                                    pj.Inventario.Add(item);

                                    if (item.Equipado)
                                    {
                                        AsignarItemEquipado(item);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar personaje:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pj = new PersonajeCompleto();
            }
        }

        private void AsignarItemEquipado(Item item)
        {
            if (item.Tipo == "Arma")
                pj.EquipoActual.Arma = item;
            else if (item.Tipo.Contains("Armadura"))
                pj.EquipoActual.Armadura = item;
            else if (item.Tipo == "Accesorio")
                pj.EquipoActual.Accesorio = item;
        }

        private void ConfigurarUI()
        {
            this.Text = "Inventario del Personaje";
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(40, 30, 20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Panel izquierdo - Estadísticas
            Panel panelIzquierdo = new Panel
            {
                Location = new Point(15, 15),
                Size = new Size(300, 580),
                BackColor = Color.FromArgb(60, 45, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            PictureBox pbImagen = new PictureBox
            {
                Location = new Point(50, 20),
                Size = new Size(200, 200),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.DimGray
            };

            Bitmap bmp = new Bitmap(200, 200);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.DimGray);
                using (var f = new Font("Papyrus", 16, FontStyle.Bold))
                using (var br = new SolidBrush(Color.White))
                {
                    g.DrawString("PERSONAJE", f, br, new PointF(35, 90));
                }
            }
            pbImagen.Image = bmp;

            lblEstadisticas = new Label
            {
                Location = new Point(15, 235),
                Size = new Size(270, 280),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.Transparent
            };

            lblOro = new Label
            {
                Location = new Point(15, 520),
                Size = new Size(270, 40),
                ForeColor = Color.Gold,
                Font = new Font("Papyrus", 14, FontStyle.Bold),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };

            panelIzquierdo.Controls.Add(pbImagen);
            panelIzquierdo.Controls.Add(lblEstadisticas);
            panelIzquierdo.Controls.Add(lblOro);

            // Panel centro - Inventario
            Panel panelCentro = new Panel
            {
                Location = new Point(330, 15),
                Size = new Size(420, 580),
                BackColor = Color.FromArgb(60, 45, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTituloInv = new Label
            {
                Text = "INVENTARIO",
                Font = new Font("Papyrus", 14, FontStyle.Bold),
                ForeColor = Color.Gold,
                Location = new Point(10, 10),
                AutoSize = true
            };

            listaItems = new ListView
            {
                Location = new Point(10, 45),
                Size = new Size(400, 420),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(240, 235, 220),
                Font = new Font("Segoe UI", 9),
                MultiSelect = false,
                HideSelection = false
            };
            listaItems.Columns.Add("Objeto", 200);
            listaItems.Columns.Add("Tipo", 100);
            listaItems.Columns.Add("Cantidad", 90);
            listaItems.SelectedIndexChanged += ListaItems_SelectedIndexChanged;

            btnEquipar = CrearBotonConIcono("⚔ EQUIPAR", 10, 480, 130);
            btnUsar = CrearBotonConIcono("🧪 USAR", 145, 480, 130);
            btnDesequipar = CrearBotonConIcono("✖ DESEQUIPAR", 280, 480, 130);

            btnEquipar.Enabled = false;
            btnUsar.Enabled = false;
            btnDesequipar.Enabled = false;

            btnEquipar.Click += BtnEquipar_Click;
            btnUsar.Click += BtnUsar_Click;
            btnDesequipar.Click += BtnDesequipar_Click;

            panelCentro.Controls.Add(lblTituloInv);
            panelCentro.Controls.Add(listaItems);
            panelCentro.Controls.Add(btnEquipar);
            panelCentro.Controls.Add(btnUsar);
            panelCentro.Controls.Add(btnDesequipar);

            // Panel derecho - Equipamiento
            panelEquipo = new Panel
            {
                Location = new Point(765, 15),
                Size = new Size(210, 580),
                BackColor = Color.FromArgb(60, 45, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTituloEquipo = new Label
            {
                Text = "EQUIPAMIENTO",
                Font = new Font("Papyrus", 11, FontStyle.Bold),
                ForeColor = Color.Gold,
                Location = new Point(10, 10),
                AutoSize = false,
                Size = new Size(190, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblArma = new Label
            {
                Text = "Arma:",
                Font = new Font("Papyrus", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 60),
                AutoSize = true
            };

            lblArmaEquipada = new Label
            {
                Text = "Ninguna",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                Location = new Point(15, 85),
                Size = new Size(180, 60),
                AutoSize = false
            };

            Label lblArmadura = new Label
            {
                Text = "Armadura:",
                Font = new Font("Papyrus", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 160),
                AutoSize = true
            };

            lblArmaduraEquipada = new Label
            {
                Text = "Ninguna",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                Location = new Point(15, 185),
                Size = new Size(180, 60),
                AutoSize = false
            };

            Label lblAccesorio = new Label
            {
                Text = "Accesorio:",
                Font = new Font("Papyrus", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 260),
                AutoSize = true
            };

            lblAccesorioEquipado = new Label
            {
                Text = "Ninguno",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                Location = new Point(15, 285),
                Size = new Size(180, 60),
                AutoSize = false
            };

            panelEquipo.Controls.Add(lblTituloEquipo);
            panelEquipo.Controls.Add(lblArma);
            panelEquipo.Controls.Add(lblArmaEquipada);
            panelEquipo.Controls.Add(lblArmadura);
            panelEquipo.Controls.Add(lblArmaduraEquipada);
            panelEquipo.Controls.Add(lblAccesorio);
            panelEquipo.Controls.Add(lblAccesorioEquipado);

            btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new Point(420, 600),
                Size = new Size(160, 40),
                Font = new Font("Papyrus", 12, FontStyle.Bold),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCerrar.FlatAppearance.BorderColor = Color.Gold;
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.Add(panelIzquierdo);
            this.Controls.Add(panelCentro);
            this.Controls.Add(panelEquipo);
            this.Controls.Add(btnCerrar);
        }

        private Button CrearBotonConIcono(string texto, int x, int y, int ancho)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(ancho, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.SaddleBrown,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.Gold;
            btn.FlatAppearance.BorderSize = 2;

            btn.MouseEnter += (s, e) =>
            {
                if (btn.Enabled) btn.BackColor = Color.FromArgb(139, 90, 43);
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = btn.Enabled ? Color.SaddleBrown : Color.FromArgb(100, 75, 50);
            };

            return btn;
        }

        private void CargarDatosUI()
        {
            if (pj == null) return;

            lblEstadisticas.Text =
                $"━━━━━━━━━━━━━━━━━━━━━\n" +
                $"  {pj.Nombre}\n" +
                $"  {pj.Clase} - Nivel {pj.Nivel}\n" +
                $"━━━━━━━━━━━━━━━━━━━━━\n\n" +
                $"ATRIBUTOS:\n\n" +
                $"  Fuerza (STR):        {pj.Fuerza}\n" +
                $"  Destreza (DEX):      {pj.Destreza}\n" +
                $"  Constitución (CON):  {pj.Constitucion}\n" +
                $"  Inteligencia (INT):  {pj.Inteligencia}\n" +
                $"  Sabiduría (WIS):     {pj.Sabiduria}\n" +
                $"  Carisma (CHA):       {pj.Carisma}\n\n" +
                $"━━━━━━━━━━━━━━━━━━━━━\n\n" +
                $"  Vida: {pj.VidaActual}/{pj.VidaMax} PV";

            lblOro.Text = $"💰 {pj.Oro} ORO";

            listaItems.Items.Clear();
            if (pj.Inventario != null)
            {
                foreach (var it in pj.Inventario)
                {
                    var itv = new ListViewItem(it.Nombre ?? "Sin nombre");
                    itv.SubItems.Add(it.Tipo ?? "Sin tipo");
                    itv.SubItems.Add(it.Cantidad.ToString());

                    // Resaltar items equipados
                    if (it.Equipado)
                    {
                        itv.BackColor = Color.LightGreen;
                        itv.Font = new Font(listaItems.Font, FontStyle.Bold);
                    }

                    itv.Tag = it;
                    listaItems.Items.Add(itv);
                }
            }

            ActualizarEquipamiento();
        }

        private void ActualizarEquipamiento()
        {
            if (pj.EquipoActual.Arma != null)
                lblArmaEquipada.Text = $"{pj.EquipoActual.Arma.Nombre}\n{pj.EquipoActual.Arma.Descripcion}";
            else
                lblArmaEquipada.Text = "Ninguna";

            if (pj.EquipoActual.Armadura != null)
                lblArmaduraEquipada.Text = $"{pj.EquipoActual.Armadura.Nombre}\n{pj.EquipoActual.Armadura.Descripcion}";
            else
                lblArmaduraEquipada.Text = "Ninguna";

            if (pj.EquipoActual.Accesorio != null)
                lblAccesorioEquipado.Text = $"{pj.EquipoActual.Accesorio.Nombre}\n{pj.EquipoActual.Accesorio.Descripcion}";
            else
                lblAccesorioEquipado.Text = "Ninguno";
        }

        private void ListaItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listaItems.SelectedItems.Count > 0)
            {
                var item = listaItems.SelectedItems[0].Tag as Item;

                if (item != null)
                {
                    bool esPocion = (item.IdObjeto >= 1 && item.IdObjeto <= 4) ||
                                   item.Nombre.ToLower().Contains("pocion") ||
                                   item.Tipo.ToLower().Contains("pocion");

                    bool esEquipable = !esPocion && (item.IdObjeto >= 10 && item.IdObjeto <= 51);
                    bool estaEquipado = item.Equipado;

                    btnUsar.Enabled = esPocion;
                    btnUsar.BackColor = btnUsar.Enabled ? Color.SaddleBrown : Color.FromArgb(100, 75, 50);

                    btnEquipar.Enabled = esEquipable && !estaEquipado;
                    btnEquipar.BackColor = btnEquipar.Enabled ? Color.SaddleBrown : Color.FromArgb(100, 75, 50);

                    btnDesequipar.Enabled = estaEquipado;
                    btnDesequipar.BackColor = btnDesequipar.Enabled ? Color.SaddleBrown : Color.FromArgb(100, 75, 50);
                }
                else
                {
                    DeshabilitarTodosBotones();
                }
            }
            else
            {
                DeshabilitarTodosBotones();
            }
        }

        private void DeshabilitarTodosBotones()
        {
            btnEquipar.Enabled = false;
            btnEquipar.BackColor = Color.FromArgb(100, 75, 50);

            btnUsar.Enabled = false;
            btnUsar.BackColor = Color.FromArgb(100, 75, 50);

            btnDesequipar.Enabled = false;
            btnDesequipar.BackColor = Color.FromArgb(100, 75, 50);
        }

        private void BtnEquipar_Click(object sender, EventArgs e)
        {
            if (listaItems.SelectedItems.Count == 0) return;

            var item = listaItems.SelectedItems[0].Tag as Item;
            if (item == null) return;

            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("CALL EquiparItem(@id, @obj, @resultado)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", jugadorId);
                        cmd.Parameters.AddWithValue("@obj", item.IdObjeto);

                        var resultadoParam = new MySqlParameter("@resultado", MySqlDbType.VarChar, 100);
                        resultadoParam.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(resultadoParam);

                        cmd.ExecuteNonQuery();

                        string resultado = resultadoParam.Value.ToString();

                        if (resultado == "Equipado exitosamente")
                        {
                            CargarPersonajeCompleto();
                            CargarDatosUI();

                            MessageBox.Show(
                                $"⚔ {item.Nombre} equipado correctamente!",
                                "Equipo actualizado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al equipar:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDesequipar_Click(object sender, EventArgs e)
        {
            if (listaItems.SelectedItems.Count == 0) return;

            var item = listaItems.SelectedItems[0].Tag as Item;
            if (item == null) return;

            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("CALL DesequiparItem(@id, @obj, @resultado)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", jugadorId);
                        cmd.Parameters.AddWithValue("@obj", item.IdObjeto);

                        var resultadoParam = new MySqlParameter("@resultado", MySqlDbType.VarChar, 100);
                        resultadoParam.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(resultadoParam);

                        cmd.ExecuteNonQuery();

                        string resultado = resultadoParam.Value.ToString();

                        if (resultado == "Desequipado exitosamente")
                        {
                            CargarPersonajeCompleto();
                            CargarDatosUI();

                            MessageBox.Show(
                                $"✖ {item.Nombre} desequipado!",
                                "Equipo actualizado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al desequipar:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUsar_Click(object sender, EventArgs e)
        {
            if (listaItems.SelectedItems.Count == 0) return;

            var item = listaItems.SelectedItems[0].Tag as Item;
            if (item == null) return;

            bool esPocion = (item.IdObjeto >= 1 && item.IdObjeto <= 4) ||
                           item.Nombre.ToLower().Contains("pocion") ||
                           item.Tipo.ToLower().Contains("pocion");

            if (!esPocion)
            {
                MessageBox.Show("Este objeto no se puede usar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("CALL UsarPocionInventario(@id, @obj, @hp_ant, @hp_nuevo, @hp_max, @resultado)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", jugadorId);
                        cmd.Parameters.AddWithValue("@obj", item.IdObjeto);

                        var hpAnteriorParam = new MySqlParameter("@hp_ant", MySqlDbType.Int32);
                        hpAnteriorParam.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(hpAnteriorParam);

                        var hpNuevoParam = new MySqlParameter("@hp_nuevo", MySqlDbType.Int32);
                        hpNuevoParam.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(hpNuevoParam);

                        var hpMaxParam = new MySqlParameter("@hp_max", MySqlDbType.Int32);
                        hpMaxParam.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(hpMaxParam);

                        var resultadoParam = new MySqlParameter("@resultado", MySqlDbType.VarChar, 100);
                        resultadoParam.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(resultadoParam);

                        cmd.ExecuteNonQuery();

                        int hpAntes = Convert.ToInt32(hpAnteriorParam.Value);
                        int hpNuevo = Convert.ToInt32(hpNuevoParam.Value);
                        int hpMax = Convert.ToInt32(hpMaxParam.Value);
                        int vidaCurada = hpNuevo - hpAntes;
                        string resultado = resultadoParam.Value.ToString();

                        if (resultado == "Poción usada exitosamente")
                        {
                            pj.VidaActual = hpNuevo;
                            pj.VidaMax = hpMax;

                            CargarPersonajeCompleto();
                            CargarDatosUI();

                            MessageBox.Show(
                                $"¡Has usado {item.Nombre}!\n\n" +
                                $"Vida restaurada: +{vidaCurada} PV\n" +
                                $"Vida actual: {hpNuevo}/{hpMax} PV",
                                "Poción consumida",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        else
                        {
                            MessageBox.Show(resultado, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al usar el objeto:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Clases de datos
        public class Item
        {
            public int IdObjeto { get; set; }
            public string Nombre { get; set; }
            public string Tipo { get; set; }
            public string Categoria { get; set; }
            public string Descripcion { get; set; }
            public int BonVida { get; set; }
            public int BonFuerza { get; set; }
            public int BonDestreza { get; set; }
            public int Cantidad { get; set; } = 1;
            public bool Equipado { get; set; } = false;
        }

        public class Equipo
        {
            public Item Arma { get; set; }
            public Item Armadura { get; set; }
            public Item Accesorio { get; set; }
        }

        public class PersonajeCompleto
        {
            public string Nombre { get; set; } = "Sin nombre";
            public string Clase { get; set; } = "Aventurero";
            public int Nivel { get; set; } = 1;
            public int Fuerza { get; set; } = 10;
            public int Destreza { get; set; } = 10;
            public int Constitucion { get; set; } = 10;
            public int Inteligencia { get; set; } = 10;
            public int Sabiduria { get; set; } = 10;
            public int Carisma { get; set; } = 10;
            public int VidaActual { get; set; } = 10;
            public int VidaMax { get; set; } = 10;
            public int Oro { get; set; } = 0;
            public Equipo EquipoActual { get; set; } = new Equipo();
            public List<Item> Inventario { get; set; } = new List<Item>();
            public Image ImagenPersonaje { get; set; }
        }
    }
}