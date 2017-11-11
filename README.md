# Demo
该仓库用于存储技术Demo代码。

## Demo列表
- [XmlSerializerDemo](#xmlserializerdemo)：Xml Serializer序列化示例，支持未知类型

### XmlSerializerDemo
Xml Serializer序列化示例，基于[XSerializer](https://github.com/QuickenLoans/XSerializer)实现。
#### 基于XSerializer的特性  
* 支持抽象类型，包括基类和接口
* 不需要通过`XmlInclude`在类型定义中预先标记未知类型
* 支持XmlSerializer序列化中的序列化自定义特性，[XmlRoot], [XmlElement], [XmlAttribute]等。
#### 关键序列化代码
``` CSharp
XmlSerializer<RootType> serializer = new XmlSerializer<RootType>(
    unknownTyeps //未知类型，如果有其他扩展类型也需要加入
    );
//序列化为字符串
string xml = serializer.Serialize(rootTypeObj);
//反序列化
RootType roundTripFoo = serializer.Deserialize(xml);
```


<a target='_blank' rel='nofollow' href='https://app.codesponsor.io/link/q6NFtNujicHJPWrvRTPNrD5i/zenglo/demo'>
  <img alt='Sponsor' width='888' height='68' src='https://app.codesponsor.io/embed/q6NFtNujicHJPWrvRTPNrD5i/zenglo/demo.svg' />
</a>
