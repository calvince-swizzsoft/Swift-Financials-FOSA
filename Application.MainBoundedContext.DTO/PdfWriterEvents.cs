using Infrastructure.Crosscutting.Framework.Utils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;

namespace Application.MainBoundedContext.DTO
{
    public class PdfWriterEvents : PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate template;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;

        AssemblyAttributes _assemblyAttributes = null;

        #region Properties

        private byte[] _logo;
        public byte[] Logo
        {
            get { return _logo; }
            set { _logo = value; }
        }

        private string _headerRow1;
        public string HeaderRow1
        {
            get { return _headerRow1; }
            set { _headerRow1 = value; }
        }

        private string _headerRow2;
        public string HeaderRow2
        {
            get { return _headerRow2; }
            set { _headerRow2 = value; }
        }

        private Font _headerFont;
        public Font HeaderFont
        {
            get { return _headerFont; }
            set { _headerFont = value; }
        }

        private Font _footerFont;
        public Font FooterFont
        {
            get { return _footerFont; }
            set { _footerFont = value; }
        }

        private int _lastPage;
        public int LastPage
        {
            get { return _lastPage; }
            private set { _lastPage = value; }
        }

        private string _watermarkText;
        public string WatermarkText
        {
            get { return _watermarkText; }
            private set { _watermarkText = value; }
        }

        private string _applicationUserName;
        public string ApplicationUserName
        {
            get { return _applicationUserName; }
            private set { _applicationUserName = value; }
        }

        #endregion

        public PdfWriterEvents(string watermarkText, string applicationUserName)
        {
            _watermarkText = watermarkText;
            _applicationUserName = applicationUserName;
            _assemblyAttributes = new AssemblyAttributes();
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException)
            { }
            catch (System.IO.IOException)
            { }
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            Rectangle pageSize = document.PageSize;

            if (!string.IsNullOrWhiteSpace(HeaderRow1 + HeaderRow2))
            {
                PdfPTable headerTable = new PdfPTable(new float[] { 1f, 2f, 2f });
                headerTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                headerTable.TotalWidth = document.Right - (document.Right * 0.15f);

                PdfPCell headerLeftCell = Logo != null ? new PdfPCell(iTextSharp.text.Image.GetInstance(Logo), true) : new PdfPCell(new Phrase(8, "[LOGO_HERE]", HeaderFont));
                headerLeftCell.Padding = 5;
                headerLeftCell.PaddingBottom = 8;
                headerLeftCell.BorderWidthRight = 0;
                headerLeftCell.Rowspan = 2;
                headerTable.AddCell(headerLeftCell);

                PdfPCell headerRightCell_0 = new PdfPCell(new Phrase(8, HeaderRow1, HeaderFont));
                headerRightCell_0.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                headerRightCell_0.Padding = 5;
                headerRightCell_0.PaddingBottom = 8;
                headerRightCell_0.BorderWidthLeft = 0;
                headerRightCell_0.Colspan = 2;
                headerTable.AddCell(headerRightCell_0);

                PdfPCell headerRightCell_1 = new PdfPCell(new Phrase(8, HeaderRow2, HeaderFont));
                headerRightCell_1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                headerRightCell_1.Padding = 5;
                headerRightCell_1.PaddingBottom = 8;
                headerRightCell_1.BorderWidthLeft = 0;
                headerRightCell_1.Colspan = 2;
                headerTable.AddCell(headerRightCell_1);

                cb.SetRGBColorFill(0, 0, 0);
                headerTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(25), cb);
            }

            if (!string.IsNullOrWhiteSpace(WatermarkText))
            {
                float fontSize = 80;
                float xPosition = 300;
                float yPosition = 400;
                float angle = 45;
                try
                {
                    PdfContentByte under = writer.DirectContentUnder;
                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                    under.BeginText();
                    under.SetColorFill(BaseColor.LIGHT_GRAY);
                    under.SetFontAndSize(baseFont, fontSize);
                    under.ShowTextAligned(PdfContentByte.ALIGN_CENTER, WatermarkText, xPosition, yPosition, angle);
                    under.EndText();
                }
                catch (Exception)
                {
                }
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            String text = "Page " + pageN + " of ";
            float len = bf.GetWidthPoint(text, 8);

            Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();

            cb.AddTemplate(template, pageSize.GetLeft(40) + len, pageSize.GetBottom(30));

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, string.Format("Printed by {0} from {1} on {2}", ApplicationUserName, _assemblyAttributes.Product, PrintTime.ToString("dd/MM/yyyy HH:mm:ss tt")), pageSize.GetRight(40), pageSize.GetBottom(30), 0);
            cb.EndText();

            LastPage = pageN;
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber));
            template.EndText();
        }
    }
}
