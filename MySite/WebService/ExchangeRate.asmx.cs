using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace MySite.WebService
{
    /// <summary>
    /// Сводное описание для ExchangeRate
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class ExchangeRate : System.Web.Services.WebService
    {
        public static string usdValue;
        public static string euroValue;

        [WebMethod]
        public static string GetCurs()
        {
            try
            {


                //загружаем XML документ с сайта центрального банка
                XmlTextReader reader = new XmlTextReader("http://www.cbr.ru/scripts/XML_daily.asp");
                //В эти переменные будем сохранять куски XML
                //с определенными валютами (Euro, USD)
                string USDXml = "";
                string EuroXML = "";
                //Перебираем все узлы в загруженном документе
                while (reader.Read())
                {
                    //Проверяем тип текущего узла
                    switch (reader.NodeType)
                    {
                        //Если этого элемент Valute, то начинаем анализировать атрибуты
                        case XmlNodeType.Element:
                            {
                                if (reader.Name == "Valute")
                                {
                                    if (reader.HasAttributes)
                                    {
                                        //Метод передвигает указатель к следующему атрибуту
                                        while (reader.MoveToNextAttribute())
                                        {
                                            if (reader.Name == "ID")
                                            {
                                                //Если значение атрибута равно R01235, то перед нами информация о курсе доллара
                                                if (reader.Value == "R01235")
                                                {
                                                    //Возвращаемся к элементу, содержащий текущий узел атрибута
                                                    reader.MoveToElement();
                                                    //Считываем содержимое дочерних узлом
                                                    USDXml = reader.ReadOuterXml();
                                                }
                                            }

                                            //Аналогичную процедуру делаем для ЕВРО
                                            if (reader.Name == "ID")
                                            {
                                                if (reader.Value == "R01239")
                                                {
                                                    reader.MoveToElement();
                                                    EuroXML = reader.ReadOuterXml();
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }

                //Из выдернутых кусков XML кода создаем новые XML документы
                XmlDocument usdXmlDocument = new XmlDocument();
                usdXmlDocument.LoadXml(USDXml);
                XmlDocument euroXmlDocument = new XmlDocument();
                euroXmlDocument.LoadXml(EuroXML);
                //Метод возвращает узел, соответствующий выражению XPath
                XmlNode xmlNode = usdXmlDocument.SelectSingleNode("Valute/Value");

                //Считываем значение и конвертируем в decimal. Курс валют получен
                usdValue = xmlNode.InnerText;

                xmlNode = euroXmlDocument.SelectSingleNode("Valute/Value");
                euroValue = xmlNode.InnerText;
            }catch(Exception exc)
            {
                if (usdValue == null) usdValue = "Error";
                if (euroValue == null) euroValue = "Error";
                string Msg;
                Msg = exc.Message;
            }
            return "";
        }
    }
}
