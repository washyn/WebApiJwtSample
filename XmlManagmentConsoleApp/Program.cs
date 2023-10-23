using System.Xml;
using System.Xml.Linq;

var doc = "71449234";

var content = $$"""
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope
  xmlns:soap="http://www.w3.org/2003/05/soap-envelope"
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
  xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    <soap:Body>
        <obtenerDatosBasicosResponse xmlns="http://tempuri.org/">
            <obtenerDatosBasicosResult>
                <string>0000</string>
                <string />
                <string>SANTANDER</string>
                <string>VALERO</string>
                <string>SIN DATOS</string>
                <string>ALEX SADAHAN</string>
                <string>92</string>
                <string>33</string>
                <string>20</string>
                <string>09</string>
                <string>01</string>
                <string>000</string>
                <string>AMERICA</string>
                <string>PERU</string>
                <string>PUNO</string>
                <string>SAN ROMAN</string>
                <string>JULIACA</string>
                <string>SIN DATOS</string>
                <string>URB. EL CARMEN S-11</string>
                <string>1</string>
                <string>15/05/2000</string>
                <string>01/06/2017</string>
                <string>71449245</string>
            </obtenerDatosBasicosResult>
        </obtenerDatosBasicosResponse>
    </soap:Body>
</soap:Envelope>
""";


// final_data = data_dict.get('soap:Envelope', {})\
// .get('soap:Body', {})\
// .get('obtenerDatosBasicosResponse', {})\
// .get('obtenerDatosBasicosResult', {})\
// .get('string', [])

var xmlDocument = new XmlDocument();
xmlDocument.LoadXml(content);
var temp = xmlDocument.GetElementsByTagName("string");
Console.WriteLine( temp.Item(2)?.InnerXml);
Console.WriteLine( temp.Item(3)?.InnerXml);
Console.WriteLine( temp.Item(5)?.InnerXml);
Console.WriteLine( temp.Item(20)?.InnerXml);
Console.WriteLine( temp.Item(22)?.InnerXml);
Console.WriteLine("----------------------------");
Console.WriteLine( temp.Item(4)?.InnerXml);
Console.WriteLine( temp.Item(5)?.InnerXml);
Console.WriteLine( temp.Item(7)?.InnerXml);
Console.WriteLine( temp.Item(29)?.InnerXml);
Console.WriteLine( temp.Item(2)?.InnerXml);
Console.WriteLine( temp.Item(30)?.InnerXml);
Console.WriteLine( temp.Item(31)?.InnerXml);