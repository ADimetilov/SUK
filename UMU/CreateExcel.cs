using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Media;
using Windows.Data.Text;
using Excel = Microsoft.Office.Interop.Excel;
namespace UMU
{
    public class CreateExcel
    {
        private string date;
        private Excel.Application app = null;
        private Excel.Workbook workbook = null;
        private Excel.Worksheet worksheet = null;
        private Excel.Range workSheet_range = null;
        public CreateExcel()
        {
            createDoc();
        }
        public void createDoc()
        {
            try
            {
                app = new Excel.Application();
                app.Visible = true;
                workbook = app.Workbooks.Add(1);
                worksheet = (Excel.Worksheet)workbook.Sheets[1];
                worksheet.PageSetup.Zoom = 73;
            }
            catch (Exception e)
            {
                Console.Write("Error");
            }
            finally
            {
            }
        }
        public void SetTitle()
        {
            worksheet.Columns["A:A"].ColumnWidth = 7.43;
            worksheet.Columns["B:B"].ColumnWidth = 19.71;
            worksheet.Columns["C:C"].ColumnWidth = 18.29;
            worksheet.Columns["D:D"].ColumnWidth = 24.57;
            worksheet.Columns["E:E"].ColumnWidth = 22.71;
            worksheet.Columns["F:F"].ColumnWidth = 21.71;
            worksheet.Rows["1:1"].RowHeight = 39.75;
            worksheet.Rows["2:2"].RowHeight = 15;
            worksheet.Rows["3:3"].RowHeight = 3;
            worksheet.Rows["4:4"].RowHeight = 34.5;
            worksheet.Rows["5:5"].RowHeight = 45;
            worksheet.Rows["6:6"].RowHeight = 16.5;
            worksheet.Rows["7:7"].RowHeight = 23.25;
            worksheet.Rows["8:8"].RowHeight = 54;
            Excel.Range range1 = worksheet.get_Range("C2", "F2");
            range1.Merge(Type.Missing);
            range1.Style.WrapText = false;
            worksheet.Cells[2, 3] = "Приложение к Контракту от «01» декабря  2025г. № 798/0817200000325018904";
            Excel.Range range2 = worksheet.get_Range("A4", "F4");
            range2.Font.Size = 14;
            range2.Merge(Type.Missing);
            range2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range2.VerticalAlignment = Excel.XlVAlign.xlVAlignBottom;
            worksheet.Cells[4, 1] = "Заявка на заправку, восстановление картриджей, драм-картриджей для офисной техники \r\nКГБУЗ «ККБСМП № 2 имени З.С. Баркагана»";
            worksheet.Cells[8, 1] = "";
            worksheet.Cells[8, 2] = "Дата отправки";
            worksheet.Cells[8, 2].Style.WrapText = true;
            worksheet.Cells[8, 3] = "Штрикход (инвентарный номер)";
            worksheet.Cells[8, 3].Style.WrapText = true;
            worksheet.Cells[8, 4] = "Модель: картриджа, драм-картриджа";
            worksheet.Cells[8, 4].Style.WrapText = true;
            worksheet.Cells[8, 5] = "Причина";
            worksheet.Cells[8, 5].Style.WrapText = true;
            worksheet.Cells[8, 6] = "Адрес";
            worksheet.Cells[8, 6].Style.WrapText = true;
            set_borders(2, 6, 8);
            Excel.Range range3 = worksheet.get_Range("A8", "F8");
            range3.Font.Size = 14;
            range3.Font.Bold = true;
            range3.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            range3.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            range3.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            range3.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //range3.Borders.Color = Color.FromRgb(0, 0, 0);
            //range3.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            range3.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range3.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        }

        public void set_borders(int start,int end,int row)
        {
            for (int i = start; i <= end; i++)
            {
                worksheet.Cells[row, i].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[row, i].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[row, i].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[row, i].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            }
        }
        public void SetContent(List<cartridge_log> bases)
        {
            int last = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;
            last += 1;
            DateTime datenow = DateTime.Now;
            while (datenow.DayOfWeek != DayOfWeek.Tuesday && datenow.DayOfWeek != DayOfWeek.Thursday)
            {
                datenow = datenow.AddDays(1);
            }
            int number = 1;
            date = datenow.ToString("yyyy-MM-dd");
            foreach (cartridge_log cartridge in bases)
            {
                Excel.Range range = worksheet.get_Range($"A{last}", $"F{last}");
                range.Font.Size = 14;
                worksheet.Rows[$"{last}:{last}"].RowHeight = 19.5;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                worksheet.Cells[last, 1] = number;
                worksheet.Cells[last, 2] = date;
                worksheet.Cells[last,3] = cartridge.Серия;
                worksheet.Cells[last, 4] = cartridge.Модель;
                worksheet.Cells[last, 6] = cartridge.Адрес;
                last += 1;
                number += 1;
            }
        }

        public void SetLastContribution()
        {
            int last = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;
            last += 5;
            worksheet.Rows[$"{last}:{last}"].RowHeight = 15;
            worksheet.Cells[last, 2] = "Наименование организ.";
            worksheet.Cells[last, 2].Style.WrapText = false;
            worksheet.Cells[last, 2].WrapText = false;
            worksheet.Cells[last, 3] = "ООО \"ПРОФ-ИТ22\"";
            last += 3;
            Excel.Range range2 = worksheet.get_Range($"C{last}:C{last+1}");
            range2.Merge(Type.Missing);
            worksheet.Cells[last, 2] = "Принял в обслуживание";
            worksheet.Cells[last, 2].Style.WrapText = true;
            worksheet.Cells[last, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            worksheet.Cells[last, 2].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            worksheet.Cells[last, 3] = "__________________";
            last += 1;
            worksheet.Cells[last, 2] = "Ф.И.О";
            worksheet.Rows[$"{last}:{last}"].RowHeight = 15;
            last += 1;
            worksheet.Cells[last, 2] = "Дата";
            worksheet.Cells[last, 3] = "__________________";
            worksheet.Rows[$"{last}:{last}"].RowHeight = 15;
            last += 1;
            worksheet.Cells[last, 2] = "Подпись";
            worksheet.Cells[last, 3] = "__________________";
            worksheet.Rows[$"{last}:{last}"].RowHeight = 15;
        }
        public void SaveTable()
        {
            try
            {
                workbook.SaveAs($"\\\\10.3.6.6\\obmen_jurina\\3. IT-Картриджи\\Заявление на заправку от {date}");
            }
            catch (Exception error)
            {

            }
        }
    }
}
