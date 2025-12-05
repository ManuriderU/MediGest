using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using MediGest.Clases;

namespace MediGest.Pages
{
    public partial class Facturacion : Page
    {
        private string carpetaFacturas = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Facturaciones");

        public Facturacion()
        {
            InitializeComponent();
            CargarFacturas();
        }

        private void CargarFacturas()
        {
            if (!Directory.Exists(carpetaFacturas))
                Directory.CreateDirectory(carpetaFacturas);

            var lista = new List<FacturaItem>();
            double totalGeneral = 0, totalPendientes = 0, totalPagadas = 0;

            foreach (var archivo in Directory.GetFiles(carpetaFacturas, "*.pdf"))
            {
                double monto = ObtenerMontoDePDF(archivo);
                string estado = "Pendiente";

                lista.Add(new FacturaItem
                {
                    Nombre = System.IO.Path.GetFileName(archivo),
                    Estado = estado,
                    Monto = monto
                });

                totalGeneral += monto;
                if (estado == "Pendiente") totalPendientes += monto;
                else if (estado == "Pagada") totalPagadas += monto;
            }

            lstFacturas.ItemsSource = lista;
            txtTotalIngresos.Text = $"{totalGeneral:F2} €";
            txtPendientes.Text = $"{totalPendientes:F2} €";
            txtPagadas.Text = $"{totalPagadas:F2} €";
        }

        private double ObtenerMontoDePDF(string rutaPDF)
        {
            double monto = 0;

            try
            {
                using (var reader = new iTextSharp.text.pdf.PdfReader(rutaPDF))
                {
                    StringBuilder texto = new StringBuilder();

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        texto.Append(iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i));
                    }

                    string contenido = texto.ToString();

                    // 🔹 Buscamos la línea que contiene "TOTAL A PAGAR"
                    var match = System.Text.RegularExpressions.Regex.Match(
                        contenido,
                        @"TOTAL A PAGAR:\s*([\d,.]+)",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );

                    if (match.Success)
                    {
                        string valor = match.Groups[1].Value
                            .Replace(",", ".")
                            .Trim();

                        if (double.TryParse(valor, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out double montoEncontrado))
                        {
                            monto = montoEncontrado;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error leyendo {System.IO.Path.GetFileName(rutaPDF)}:\n{ex.Message}");
            }

            return monto;
        }


        private void lstFacturas_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SessionManager.Rol == "Medico") {
                MessageBox.Show("No tienes permiso para cambiar el estado de las Facturas");
                return;
            }

            if (lstFacturas.SelectedItem is FacturaItem facturaSeleccionada)
            {
                // Mostrar opciones de estado
                var opcion = MessageBox.Show(
                    $"¿Quieres marcar la factura '{facturaSeleccionada.Nombre}' como Pagada?",
                    "Cambiar Estado",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question
                );

                if (opcion == MessageBoxResult.Yes)
                    facturaSeleccionada.Estado = "Pagada";
                else if (opcion == MessageBoxResult.No)
                    facturaSeleccionada.Estado = "Pendiente";
                else
                    return;

                // Refrescar el ListView
                lstFacturas.Items.Refresh();

                // Recalcular los totales
                RecalcularTotales();
            }
        }


        private void RecalcularTotales()
        {
            double totalGeneral = 0, totalPendientes = 0, totalPagadas = 0;

            foreach (FacturaItem item in lstFacturas.Items)
            {
                totalGeneral += item.Monto;
                if (item.Estado == "Pendiente") totalPendientes += item.Monto;
                else if (item.Estado == "Pagada") totalPagadas += item.Monto;
            }

            txtTotalIngresos.Text = $"{totalGeneral:F2} €";
            txtPendientes.Text = $"{totalPendientes:F2} €";
            txtPagadas.Text = $"{totalPagadas:F2} €";
        }


        private class FacturaItem
        {
            public string Nombre { get; set; }
            public string Estado { get; set; }
            public double Monto { get; set; }
        }

       
    }
}
