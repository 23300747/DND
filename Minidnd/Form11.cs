using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Proyecto_Dnd
{
    public partial class Form11 : Form
    {
        private int jugadorId;
        private int oroActual;
        private string categoriaActual = "Pocion";
        private ListView listaProductos;
        private Label lblOro;
        private Label lblDescripcion;
        private Button btnComprar;

        public Form11(int idJugador)
        {
            InitializeComponent();
            jugadorId = idJugador;
            ConfigurarTienda();
            CargarOroJugador();
            CargarProductos("Pocion");
        }

        private void ConfigurarTienda()
        {
            this.Text = "Tienda de Zark - Retiro del Dragón";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(40, 30, 20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblTitulo = new Label
            {
                Text = "TIENDA DE ZARK",
                Font = new Font("Papyrus", 18, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = false,
                Size = new Size(860, 50),
                Location = new Point(20, 10),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblOro = new Label
            {
                Text = "Oro: 0",
                Font = new Font("Papyrus", 12, FontStyle.Bold),
                ForeColor = Color.Gold,
                Location = new Point(720, 70),
                AutoSize = true
            };

            Button btnPociones = CrearBotonCategoria("POCIONES", 20, 70);
            Button btnArmas = CrearBotonCategoria("ARMAS", 180, 70);

            btnPociones.Click += (s, e) => CambiarCategoria("Pocion");
            btnArmas.Click += (s, e) => CambiarCategoria("Arma");

            listaProductos = new ListView
            {
                Location = new Point(20, 120),
                Size = new Size(550, 360),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(240, 235, 220),
                Font = new Font("Segoe UI", 10),
                MultiSelect = false,
                HideSelection = false
            };
            listaProductos.Columns.Add("Producto", 300);
            listaProductos.Columns.Add("Precio", 120);
            listaProductos.Columns.Add("Stock", 100);
            listaProductos.SelectedIndexChanged += ListaProductos_SelectedIndexChanged;

            Panel panelInfo = new Panel
            {
                Location = new Point(590, 120),
                Size = new Size(290, 360),
                BackColor = Color.FromArgb(60, 45, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTituloDesc = new Label
            {
                Text = "DESCRIPCIÓN",
                Font = new Font("Papyrus", 11, FontStyle.Bold),
                ForeColor = Color.Gold,
                Location = new Point(10, 10),
                AutoSize = true
            };

            lblDescripcion = new Label
            {
                Text = "Selecciona un producto para ver su descripción.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.White,
                Location = new Point(10, 45),
                Size = new Size(270, 260),
                AutoSize = false
            };

            btnComprar = new Button
            {
                Text = "COMPRAR",
                Location = new Point(10, 315),
                Size = new Size(270, 35),
                Font = new Font("Papyrus", 11, FontStyle.Bold),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnComprar.FlatAppearance.BorderColor = Color.Gold;
            btnComprar.FlatAppearance.BorderSize = 2;
            btnComprar.Click += BtnComprar_Click;

            btnComprar.MouseEnter += (s, e) =>
            {
                if (btnComprar.Enabled) btnComprar.BackColor = Color.Green;
            };
            btnComprar.MouseLeave += (s, e) =>
            {
                btnComprar.BackColor = btnComprar.Enabled ? Color.DarkGreen : Color.FromArgb(60, 80, 60);
            };

            panelInfo.Controls.Add(lblTituloDesc);
            panelInfo.Controls.Add(lblDescripcion);
            panelInfo.Controls.Add(btnComprar);

            Button btnSalir = new Button
            {
                Text = "SALIR",
                Location = new Point(370, 500),
                Size = new Size(160, 45),
                Font = new Font("Papyrus", 12, FontStyle.Bold),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSalir.FlatAppearance.BorderColor = Color.Gold;
            btnSalir.FlatAppearance.BorderSize = 2;
            btnSalir.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblOro);
            this.Controls.Add(btnPociones);
            this.Controls.Add(btnArmas);
            this.Controls.Add(listaProductos);
            this.Controls.Add(panelInfo);
            this.Controls.Add(btnSalir);
        }

        private Button CrearBotonCategoria(string texto, int x, int y)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(150, 35),
                Font = new Font("Papyrus", 10, FontStyle.Bold),
                BackColor = Color.SaddleBrown,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.Gold;
            btn.FlatAppearance.BorderSize = 2;

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(139, 90, 43);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.SaddleBrown;

            return btn;
        }

        private void CambiarCategoria(string categoria)
        {
            categoriaActual = categoria;
            CargarProductos(categoria);
        }

        private void CargarOroJugador()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("CALL ObtenerOroJugador(@id)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", jugadorId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                oroActual = reader.GetInt32("Oro");
                                lblOro.Text = $"💰 {oroActual} ORO";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar oro: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarProductos(string categoria)
        {
            listaProductos.Items.Clear();
            btnComprar.Enabled = false;
            btnComprar.BackColor = Color.FromArgb(60, 80, 60);
            lblDescripcion.Text = "Selecciona un producto para ver su descripción.";

            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("CALL ObtenerStockTienda(@cat)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@cat", categoria);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string nombre = reader.GetString("Nombre");
                                int precio = reader.GetInt32("PrecioFinal");
                                int stock = reader.GetInt32("Cantidad");

                                ListViewItem item = new ListViewItem(nombre);
                                item.SubItems.Add($"{precio} oro");
                                item.SubItems.Add(stock.ToString());

                                item.Tag = new
                                {
                                    IdObjeto = reader.GetInt32("ID_Objeto"),
                                    Precio = precio,
                                    Efecto = reader.GetInt32("Efecto"),
                                    Categoria = reader.GetString("Categoria"),
                                    Tipo = reader.GetString("Tipo"),
                                    Requisitos = reader.IsDBNull(reader.GetOrdinal("Requisitos")) ? "" : reader.GetString("Requisitos")
                                };

                                listaProductos.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListaProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listaProductos.SelectedItems.Count > 0)
            {
                var item = listaProductos.SelectedItems[0];
                dynamic datos = item.Tag;

                string descripcion = $"📦 Producto: {item.SubItems[0].Text}\n\n";
                descripcion += $"🏷️ Categoría: {datos.Categoria}\n\n";
                descripcion += $"⚡ Efecto: {datos.Efecto}\n\n";
                descripcion += $"💰 Precio: {datos.Precio} oro\n\n";
                descripcion += $"📊 Stock: {item.SubItems[2].Text}";

                lblDescripcion.Text = descripcion;
                btnComprar.Enabled = true;
                btnComprar.BackColor = Color.DarkGreen;
            }
            else
            {
                lblDescripcion.Text = "Selecciona un producto para ver su descripción.";
                btnComprar.Enabled = false;
                btnComprar.BackColor = Color.FromArgb(60, 80, 60);
            }
        }

        private void BtnComprar_Click(object sender, EventArgs e)
        {
            if (listaProductos.SelectedItems.Count == 0) return;

            var item = listaProductos.SelectedItems[0];
            string nombre = item.SubItems[0].Text;
            dynamic datos = item.Tag;
            int precio = datos.Precio;

            if (oroActual < precio)
            {
                MessageBox.Show(
                    $"No tienes suficiente oro.\n\nNecesitas: {precio} oro\nTienes: {oroActual} oro",
                    "Oro insuficiente",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DialogResult result = MessageBox.Show(
                $"¿Deseas comprar {nombre}?\n\nPrecio: {precio} oro\nTu oro actual: {oroActual} oro",
                "Confirmar compra",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                RealizarCompraObjeto(datos.IdObjeto, nombre, precio, datos.Requisitos ?? "");
            }
        }

        private void RealizarCompraObjeto(int idObjeto, string nombreProducto, int precio, string requisitos)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("CALL RealizarCompraTienda(@id, @obj, @precio, @resultado)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", jugadorId);
                        cmd.Parameters.AddWithValue("@obj", idObjeto);
                        cmd.Parameters.AddWithValue("@precio", precio);

                        var resultadoParam = new MySqlParameter("@resultado", MySqlDbType.VarChar, 100);
                        resultadoParam.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(resultadoParam);

                        cmd.ExecuteNonQuery();

                        string resultado = resultadoParam.Value.ToString();

                        if (resultado == "Compra exitosa")
                        {
                            oroActual -= precio;
                            lblOro.Text = $"💰 {oroActual} ORO";

                            MessageBox.Show(
                                $"¡Compra exitosa!\n\n" +
                                $"Has adquirido: {nombreProducto}\n" +
                                $"Oro restante: {oroActual}",
                                "Compra realizada",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            CargarProductos(categoriaActual);
                        }
                        else
                        {
                            MessageBox.Show(resultado, "Error en compra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al realizar la compra:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Form11_Load(object sender, EventArgs e)
        {
        }
    }
}