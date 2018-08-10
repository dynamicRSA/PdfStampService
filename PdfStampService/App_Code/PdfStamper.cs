using System;
using System.IO;
using System.Web.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;

/// <summary>
/// This Web Service accepts a PDF and some text
/// The PDF will then be stamper on the bottom right hand corner with the text that you stipulate
/// The Stamped pdf is returned as base 64
/// </summary>
[WebService(Namespace = "http://pdfstamper.co.za/v1")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class PdfStamper : System.Web.Services.WebService
{
    public PdfStamper()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public byte[] AddSignatureAppearance(byte[] pdfToStamBytes, string stampString)
    {
        using (var ms = new MemoryStream())
        {
            try
            {
                var imageWidth = 425;
                var imageHeight = 89;
                var fontSize = 0;
                if (stampString.Length > 25)
                {
                    fontSize = 8;
                }
                else
                {
                    fontSize = 12;
                }

                //TODO: Make configurable
                string path = "emptyPath";
                string xsdFileLinux = @"/var/local/lawtrust/configs/signatureFont.ttf";
                string xsdFileWindows = @"C:/lawtrust/configs/signatureFont.ttf";

                if (File.Exists(xsdFileLinux))
                {
                    path = xsdFileLinux;
                }
                else if (File.Exists(xsdFileWindows))
                {
                    path = xsdFileWindows;
                }
                if (!path.Equals("emptyPath"))
                {
                    var reader = new iTextSharp.text.pdf.PdfReader(pdfToStamBytes);
                    var stamper = new iTextSharp.text.pdf.PdfStamper(reader, ms);
                    var rotation = reader.GetPageRotation(1);
                    var box = reader.GetPageSizeWithRotation(1);
                    var bf = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    var pcb = stamper.GetOverContent(1);
                    var f = new Font(bf, fontSize);
                    var p = new Phrase(stampString, f);

                    //TODO: Make coordinates configurable
                    if (rotation == 90)
                    {
                        //landscape
                        ColumnText.ShowTextAligned(pcb, Element.ALIGN_CENTER, p, 740, 10, 0);
                    }
                    else if (rotation == 180)
                    {
                        //normal PDF
                        ColumnText.ShowTextAligned(pcb, Element.ALIGN_CENTER, p, 500, 10, 0);
                    }
                    else if (rotation == 270)
                    {
                        //landscape
                        ColumnText.ShowTextAligned(pcb, Element.ALIGN_CENTER, p, 740, 10, 0);
                    }
                    else
                    {
                        //normal PDF
                        ColumnText.ShowTextAligned(pcb, Element.ALIGN_CENTER, p, 500, 10, 0);
                    }

                    pcb.SaveState();
                    stamper.Close();
                    reader.Close();
                    return ms.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //need error handling
                return null;
            }
        }
    }
}








