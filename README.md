# FiddlerExtend

  这是一个Fiddler的扩展，用来将指定host的请求，整体导出为*.jmx文件，可以直接使用Jmeter打开，方便录入测试脚本。
  
# 安装方法

  clone项目后，编译。将bin下面的MyFiddler.dll复制到Fiddler的安装目录：X:\Fiddler2\ImportExport
  还需要做host配置，将项目中app.config文件也一起复制到ImportExport目录下，配置  
  ```xml
  <?xml version="1.0" encoding="utf-8" ?>
  <configuration>
      <appSettings>
        <add key="host" value="your host"/>
      </appSettings>
  </configuration>
  
# 使用方法

  打开Fiddler，File->Export Sessions->All Sessions->Select Export Format
  选择JMeter，Next保存后的文件为*.jmx，直接用JMeter打开
  
