using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.DesignerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Word = Microsoft.Office.Interop.Word;
namespace UMU
{
    internal class CreateDocuments
    {
        private Word.Application winword;
        private Word.Document document;
        private object missing = System.Reflection.Missing.Value;
        public CreateDocuments() {
            winword = new Word.Application();
            winword.Visible = false;
        }
        public void insert_center(string text, int size)
        {
            document.Application.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            document.Application.Selection.Font.Size = size;
            document.Application.Selection.Font.Name = "Times new Roman";
            document.Application.Selection.TypeText(text);
            document.Application.Selection.Collapse(WdCollapseDirection.wdCollapseEnd);
        }
        public void insert_jestify(string text, int size)
        {
            document.Application.Selection.Select();
            document.Application.Selection.Font.Superscript = 0;
            document.Application.Selection.TypeText(text);
            document.Application.Selection.MoveLeft(WdUnits.wdCharacter, text.Length, true);
            document.Application.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphJustify;
            document.Application.Selection.Font.Size = size;
            document.Application.Selection.Collapse(WdCollapseDirection.wdCollapseEnd);
        }
        public void create_doc_state(List<cartridge_log> bases)
        {
            try
            {
                string[] names = new string[] {"№","Дата отправки","Модель","Адрес"};
                document = winword.Documents.Add(ref missing,ref missing,ref missing,ref missing);
                winword.Selection.PageSetup.Orientation = Word.WdOrientation.wdOrientPortrait;
                Word.Paragraph paragraph = document.Content.Paragraphs.Add(ref missing);
                insert_center($"Заявка на заправку, восстановление, картриджей для офисной техники",14);
                Word.Table table = document.Tables.Add(document.Application.Selection.Range, 1, 4,ref missing,ref missing);
                table.Borders.Enable = 1;
                foreach (Row row in table.Rows)
                {
                    foreach(Cell cell in row.Cells)
                    {
                        if (cell.RowIndex == 1)
                        {
                            cell.Range.Font.Name = "Times new Roman";
                            cell.Range.Font.Size = 12;
                            cell.Range.Text = names[cell.Column.Index-1];
                        }
                    }
                }
                foreach (cartridge_log cartridge in bases)
                {
                    table.Rows.Add();
                    Row row = table.Rows.Last;
                    Cells cells = row.Cells;
                    cells[1].Range.Text = cartridge.Серия;
                    cells[2].Range.Text = cartridge.Дата;
                    cells[3].Range.Text = cartridge.Модель;
                    cells[4].Range.Text = cartridge.Адрес;
                }
                winword.Visible = true;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        //public void create_doc_service(Service service,string organization)
        //{
        //    try
        //    {
        //        document = winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);
        //        winword.Selection.PageSetup.Orientation = Word.WdOrientation.wdOrientPortrait;
        //        Word.Paragraph paragraph1 = document.Content.Paragraphs.Add(ref missing);
        //        winword.Selection.Range.ParagraphFormat.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceSingle;
        //        insert_center($"Заявка на заправку, восстановление картриджей для офисной техники\n",16);
        //        Word.Paragraph paragraph2 = document.Content.Paragraphs.Add(ref missing);
        //        insert_jestify($"Принтер №{service.number} отправлен на ремонт из {service.cabinet} кабинета.\nКомментарий: {service.comment}\nДата отправки: {service.date_p}\nИз: {organization}",14);
                
        //        winword.Visible = true;
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Ошибка при формировании документа");
        //        winword.Visible = true;
        //    }
        //}
    }
}
