   M S B A M L   `   `   `  ����U  QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf  5   	   C ?PortTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:PortTestPortTest 5   	      PortTest.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Dlvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf     5   	   (localclr-namespace:PortTest     5   	     � Title$  
MainWindow��5	   	   $	��450��6   $	��800��6)    � WindowStartupLocation$ CenterScreen=�65   .��5
      �  � ColumnDefinitions 5   
   �� 5      $	��275 �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      .+�5   
   ��  �� Orientation$ Vertical=�6   $��0��6,   $��0��69   .+�5      �� $ 
Horizontal=�6   .+�5      �� -   5         ��Name$ scanDeviceBtn��5      $��扫描设备��60   $��
15 10 0 10q�6?   $��60��6S   6^   �� 5      -   5         ��Name$ 
deviceList��5      $	��175��6/   $��10 10 10 10q�6;   5      �� 5      $��体温��6   $��
10 0 10 10q�6#   .��5      �� $ Vertical=�6   .+�5      �� $ 
Horizontal=�6!   .+�5      �� $��温度值：��6    �� 5       -   5         ��Name$ curDataLabel��5        5!      �� 5"      $ 
Horizontal=�6!   .+�5#      �� $��	校准：��6    �� 5$      -   5        $ upTemBtn��5$   !   $��50��63   $��0 0 10 0q�6>   $��上调��6P   6]   �� 5%      -   5        $ 
downTemBtn��5%   !   $��50��65   $��0 0 10 0q�6@   $��下调��6R   6_   �� 5&      -   5        $ temDeltaLabel��5&       $��+0℃��67   5'      5(      5)      �� 5+      $��脉搏��6   $��
10 0 10 10q�6#   .��5,      �� $ Vertical=�6   .+�5-      �� $ 
Horizontal=�6!   .+�5.      �� $��压力值：��6    �� 5/      -   5        $ curDataLabel1��5/       50      �� 51      $ 
Horizontal=�6!   .+�52      �� $��	调幅：��6    �� 53      -   5        $ 
pulLevList��53   #   $��50��67   .@�54      �� $��0��6+   �� 55      $��1��6+   �� 56      $��2��6+   �� 57      $��3��6+   �� 58      $��4��6+   �� 59      $��5��6+   �� 5:      $��6��6+   �� 5;      $��7��6+   �� 5<      $��8��6+   �� 5=      $��9��6+   �� 5>      $��10��6+   5?      5@      5A      5B      �� 5D      $��血氧��6   $��
10 0 10 10q�6#   .��5E      �� $ Vertical=�6   .+�5F      �� $ 
Horizontal=�6!   .+�5G      �� $��	心率：��6    �� 5H      -	   5        $ curDataLabel2��5H       5I      �� 5J      $ 
Horizontal=�6!   .+�5K      �� $��血氧饱和度：��6    �� 5L      -
   5        $ curDataLabel3��5L       5M      �� 5N      $ 
Horizontal=�6!   .+�5O      �� $$��血容积脉搏波形幅值：��6    �� 5P      -   5        $ curDataLabel4��5P       5Q      5R      5S      �� 5V      -   5        $ startCaptBtn��5V      $��
15 0 15 10q�6+   $��开始监测��6?   6N   �� 5W      -   5        $ stopCaptBtn��5W      $��	15 0 15 0q�6*   $��停止监测��6=   6L   5X      �� 5[   
   $ Vertical=�6   $��0��6,   $��1��69   .+�5\      #   LiveCharts.Wpf.CartesianChart  -   5        $��	waveChart��5\   !   &    LiveCharts.Wpf.Charts.Base.Chart   Series 62   �� *SeriesCollection+5]      5^      5_      