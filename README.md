# Demo
用于存储技术Demo代码。

## Demo列表
- [XmlSerializerDemo](#xmlserializerdemo)：Xml Serializer序列化示例，支持未知类型
- [EdgeJsDemo](#edgejsdemo)：在Asp.net 中调用[Edge.Js](https://github.com/tjanczuk/edge)的示例

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

### EdgeJsDemo
[Edge.Js](https://github.com/tjanczuk/edge) v8.2.1 在控制台程序中调用实验通过，在Asp.net中调用会抛出AccessViolationException异常，而官方文档中指出Asp.net和控制台使用方式一样，从[Edge Issues](https://github.com/tjanczuk/edge/issues?utf8=%E2%9C%93&q=AccessViolationException) 来看应该是Edge的一个bug。  
在本示例中，Asp.net请求中每次执行js时通过一个控制台程序壳来间接调用Edge，从而绕开Asp.net直接调用Edge时的AccessViolationException异常，当然，这种方式会具有一定的性能损失和一定的功能局限性，但对于性能不高的简单调用倒也是一种解决方案。

<a target='_blank' rel='nofollow' href='https://app.codesponsor.io/link/q6NFtNujicHJPWrvRTPNrD5i/zenglo/demo'>
  <img alt='Sponsor' width='888' height='68' src='https://app.codesponsor.io/embed/q6NFtNujicHJPWrvRTPNrD5i/zenglo/demo.svg' />
</a>
