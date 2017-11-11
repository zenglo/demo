using Atom.Framework.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XSerializer;

namespace XmlSerializerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Foo foo = new Foo
            {
                Id = 1,
                IgnoreValue = "忽略的值",
                Date = DateTime.Now,
                Bars = new List<BaseBar> {
                    new ABar(){
                        Id =2, IgnoreValue="基忽略" , Date = DateTime.Now, Value="A值",
                        AIgnoreValue ="A忽略", APro="A元素"
                    },
                    new BBar(){
                        Id =3, IgnoreValue="基忽略" , Date = DateTime.Now, Value="B值",
                        BIgnoreValue ="B忽略", BPro="B属性"
                    }
                }
            };

            //反射扫描未知的BaseBar子类类型
            Type[] unknownTyeps = AssemblyHelper.
                RecursionNoParaConstructorClassTypes<BaseBar>(
                    AppDomain.CurrentDomain.BaseDirectory).ToArray();

            XmlSerializer<Foo> serializer = new XmlSerializer<Foo>(
                new XmlSerializationOptions(shouldIndent: true), //是否格式化xml，需要良好的xml格式时可以设置为true
                unknownTyeps //BaseBar子类类型，如果有其他扩展类型也需要加入
                );

            using (FileStream stream = new FileStream("text.xml", FileMode.Create, FileAccess.Write))
            {
                //序列化到文件流
                serializer.Serialize(stream, foo);
            }
            //序列化为字符串
            string xml = serializer.Serialize(foo);
            //反序列化
            Foo roundTripFoo = serializer.Deserialize(xml);
        }
    }
}
