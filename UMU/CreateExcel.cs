using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Excel = Microsoft.Office.Interop.Excel;
namespace UMU
{
    public class CreateExcel
    {
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
            worksheet.Cells[1, 1] = "Дата отправки";
            worksheet.Cells[1, 2] = "Штрикход (инвентарный номер)";
            worksheet.Cells[1, 3] = "Модель";
            worksheet.Cells[1, 4] = "Причина";
            worksheet.Cells[1, 5] = "Адрес";
        }
        public void SetContent(List<cartridge_log> bases)
        {
            int last = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;
            last += 1;
            foreach (cartridge_log cartridge in bases)
            {
                worksheet.Cells[last, 1] = cartridge.Дата;
                worksheet.Cells[last,2] = cartridge.Серия;
                worksheet.Cells[last, 3] = cartridge.Модель;
                worksheet.Cells[last, 5] = cartridge.Адрес;
                last += 1;
            }
        }
        public void SaveTable()
        {
            workbook.SaveAs($"\\\\10.3.6.6\\obmen_jurina\\3. IT-Картриджи\\Заявку на заправку от {DateTime.Now.ToString("yyyy-dd-MM")}");
        }
    }
}
