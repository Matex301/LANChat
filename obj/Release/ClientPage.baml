   M S B A M L   `   `   `  ���� B  >LANChat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:LANChatLANChat      LANChat.ClientPage   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     'localclr-namespace:LANChat       N� Title$  
ClientPage��.��� .+��� -   $��ChatContainer��$
��Left=�$	��550��$��	10,10,0,0q�$	��Top=�$	��800������ TempBox��.R���  �� Orientation$ 
Horizontal=�.+��� ���� *	BackColor+���� *
InerText+�� -   $��Input�� �� 	MaxLength$	 120��$
��Left=�$��40��$��
10,565,0,0q� �� TextWrapping$
 Wrap=�$��4q�$	��Top=�$	��800��$��24(�$��Italic$�.���� -   $��	InputNick��$ 36��$��16(�$��	0,99,25,0q�$
 Wrap=�$�� ��$��Right=�$	��122��$��28��$	��Top=��� -   $��Start��$��Start��$��
0,159,25,0q�$��Right=�$	��Top=�$��75��$��20(��� $
��IP: ��$��
0,66,170,0q�$	��Top=�$��28��$��Right=�$��26���� $��Nick: ��$��Right=�$��
0,99,170,0q�$	��Top=�$��28���� -   $��StatusLabel��
���  ��$��Disconnected��$��
0,30,105,0q�$��14(�$��Right=�$	��Top=�$��31��$	��102���� -   $��NotificationsBox��$��Notifications��$��
0,138,25,0q�$��Right=�$	��105��$��16��$	��Top=� d� 	IsChecked$
 TrueY��� -   $��
ServerList��$��885,66,25,0q�$	��Top=�$��24��$��12(����� .R��� �� ��� *Text+�� -   $��RefreshButton��$��Refresh��$��945,29,25,0q�$	��Top=�$��31��