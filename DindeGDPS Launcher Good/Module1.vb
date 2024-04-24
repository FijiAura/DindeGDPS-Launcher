Imports System.IO
Imports System.Net
Imports System.Threading
Imports System.Xml
Imports Microsoft.Win32
Imports System.IO.Pipes
Imports System.Threading.Tasks
Public Module Module1
    Public Sub ChangeControlColors(ByVal container As Control.ControlCollection, ByVal colorName As String)
        For Each ctrl As Control In container
            If TypeOf ctrl Is LinkLabel Then
                CType(ctrl, LinkLabel).ForeColor = Color.FromName(colorName)
                CType(ctrl, LinkLabel).LinkColor = Color.FromName(colorName)
            ElseIf TypeOf ctrl Is Label OrElse TypeOf ctrl Is Button OrElse TypeOf ctrl Is ComboBox OrElse TypeOf ctrl Is CheckBox OrElse TypeOf ctrl Is TextBox Then
                ctrl.ForeColor = Color.FromName(colorName)
            ElseIf TypeOf ctrl Is ComboBox Then
                CType(ctrl, ComboBox).ForeColor = Color.FromName(colorName)
            ElseIf TypeOf ctrl Is CheckBox Then
                CType(ctrl, CheckBox).ForeColor = Color.FromName(colorName)
            End If
        Next
    End Sub

    Public Sub ChangeMenuStripColors(ByVal container As Control.ControlCollection, ByVal colorName As String)
        For Each Control As Control In container
            If Control.GetType.ToString = "System.Windows.Forms.MenuStrip" Then
                Dim MenuStrip As New MenuStrip
                MenuStrip = CType(Control, MenuStrip)
                For Each ToolStripMenuItem As ToolStripMenuItem In MenuStrip.Items
                    ToolStripMenuItem.ForeColor = Color.FromName(colorName)
                    For Each DropDownItem As ToolStripDropDownItem In ToolStripMenuItem.DropDownItems
                        DropDownItem.ForeColor = Color.FromName(colorName)
                    Next
                Next
            End If
        Next
    End Sub

    ' wtf am i doing
    Private listenerTask As Task

    Public RootFS = AppDomain.CurrentDomain.BaseDirectory
    Public nl = Environment.NewLine

    Public Sub OpenWait(f As Form)
        f.ShowDialog()
    End Sub

    Async Function JeMoeder(u As String) As Task(Of String)
        ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)
        Dim res As String
        ' Create a web request
        Dim request As HttpWebRequest = CType(WebRequest.Create(u), HttpWebRequest)

        ' Get the response
        Dim response As HttpWebResponse = CType(Await request.GetResponseAsync(), HttpWebResponse)

        ' Read the response stream
        Using streamReader As New StreamReader(response.GetResponseStream())
            ' Read the first line of the response
            res = streamReader.ReadLine()

            ' Handle the first line as needed
            Console.WriteLine("First line of response: " & res)
        End Using

        ' Close the response
        response.Close()

        Return res
    End Function
    Public Function IsOnlyInstance() As Boolean
        ' Check if another instance is running
        Dim processes As Process() = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)
        Return processes.Length = 1
    End Function

    Public Function DownloadXmlContent(url As String) As String
        Try
            ' Create a WebClient to download the XML content
            Using webClient As New WebClient()
                ' Download the content as a string
                Return webClient.DownloadString(url)
            End Using
        Catch ex As Exception
            ' Handle exceptions, such as network errors
            MsgBox($"Error downloading XML content: {ex.Message}", vbOKOnly + vbCritical, "Custom GDPS")
            Return String.Empty
        End Try
    End Function

    Public Async Function GDPSVerCheck(gdps As String, UpdateBypass As Boolean) As Tasks.Task(Of Boolean)

        If My.Settings.DisableUpd = True And UpdateBypass = False Then
            Return True
        End If
        Dim fp As String
        If File.Exists(Path.Combine(RootFS, gdps, "info.xml")) Then
            fp = Path.Combine(RootFS, gdps, "info.xml")
        Else
            Return True
        End If
        Dim xml As New XmlDocument()
        xml.Load(fp)
        Dim config As XmlElement = xml.DocumentElement
        Dim lver = config.SelectSingleNode("version").InnerText

        Dim updateUrlNode As XmlNode = config.SelectSingleNode("updateurl")
        Dim updateurl As String = If(updateUrlNode IsNot Nothing AndAlso Not String.IsNullOrEmpty(updateUrlNode.InnerText) AndAlso updateUrlNode.InnerText <> "default",
                                   updateUrlNode.InnerText,
                                   $"https://v2.dgdps.us.to/?client=game&gv={gdps}")



        ' chaos => Dim updateurl = If(String.IsNullOrEmpty(config.SelectSingleNode("updateurl").InnerText) Or config.SelectSingleNode("updateurl").InnerText = "default", $"https://v2.dgdps.us.to/?client=game&gv={gdps}", config.SelectSingleNode("updateurl").InnerText)

        ServicePointManager.SecurityProtocol = DirectCast(3072, System.Net.SecurityProtocolType)
        Dim updaterchk As New WebClient
        Dim over As String = Await JeMoeder(updateurl)

        If lver = over Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function GetDirKeyword(directoryPath As String) As String()
        ' Check if the directory exists
        If Directory.Exists(directoryPath) Then
            ' Get all directories in the specified path
            Dim allDirectories As String() = Directory.GetDirectories(directoryPath)

            ' Filter directories that contain an info.xml file
            Dim filteredDirectories As IEnumerable(Of String) = allDirectories.
            Where(Function(dir) File.Exists(Path.Combine(dir, "info.xml")))

            ' Extract only the directory names without the absolute path
            Dim directoryArray As String() = filteredDirectories.
            Select(Function(dir) Path.GetFileName(dir)).ToArray()

            Return directoryArray
        Else
            ' Handle the case when the directory doesn't exist
            MessageBox.Show("Directory does not exist.")
            Return New String() {}
        End If
    End Function

    Async Function dogcheck() As Tasks.Task
        Dim dog = False
        ' Stack to keep track of directories to be checked
        Dim directoriesToCheck As New Stack(Of String)()
        directoriesToCheck.Push(RootFS)

        ' Continue checking until all directories are processed
        While directoriesToCheck.Count > 0
            Dim currentDirectory As String = directoriesToCheck.Pop()

            ' Check if the "Resource" folder exists in the current directory
            If File.Exists(Path.Combine(currentDirectory, "info.xml")) Then
                dog = True
                Dim fp = Path.Combine(currentDirectory, "info.xml")
                Dim xml As New XmlDocument()
                xml.Load(fp)
                Dim config As XmlElement = xml.DocumentElement
                Dim gdpsname = config.SelectSingleNode("game").InnerText
                Dim test = Await GDPSVerCheck(gdpsname, False)

                If test = False Then
                    Select Case gdpsname
                        Case "gd19"
                            MsgBox("DindeGDPS 1.9 got a new update! Check ""Instance Manager"" in the ""GDPS"" tab :D")
                        Case "gd20"
                            MsgBox("DindeGDPS 2.0 got a new update! Check ""Instance Manager"" in the ""GDPS"" tab :D")
                        Case "gd21"
                            MsgBox("DindeGDPS 2.1 got a new update! Check ""Instance Manager"" in the ""GDPS"" tab :D")
                        Case "gd22"
                            MsgBox("DindeGDPS 2.2 got a new update! Check ""Instance Manager"" in the ""GDPS"" tab :D")
                        Case Else
                            MsgBox($"{gdpsname} got a new update! Go to the GDPS' download page or ask the owner for the link of the update!")
                    End Select
                End If
            End If

            ' Get subdirectories and add them to the stack for further checking
            Try
                Dim subDirectories As String() = Directory.GetDirectories(currentDirectory)
                For Each subDirectory As String In subDirectories
                    directoriesToCheck.Push(subDirectory)
                Next
            Catch ex As Exception
                ' Handle exceptions, if any
                Console.WriteLine("Error: " & ex.Message)
            End Try
        End While
        ' hold on: he doesn't have a gdps?
        If dog = False Then
            Form4.Show()
            MsgBox("No GDPS instances have been detected! You can download one by clicking on ""Install""", vbOKOnly + vbExclamation, "No instances found")
        End If
    End Function

    Public Function RefreshGDPS()
        ' Get an array of directory names that start with "gd"
        Dim directoryArray As String() = GetDirKeyword(RootFS)
        ' Populate the ComboBox with the filtered directory names
        Return directoryArray
    End Function

    Public Function RegisterProtocol()

        Dim protocolName As String = "dgdps"
        Dim applicationPath As String = Application.ExecutablePath
        Dim description As String = "DindeGDPS Launcher"

        If My.Settings.CustomGDPS = True Then
            Dim a = MsgBox("The custom GDPS option has already been enabled!" + nl + "Disable it?", vbYesNo + vbQuestion, "Custom GDPS")
            If a = vbNo Then
                Return True
            End If
            ' Delete the keys associated with the custom protocol
            Dim keyPath0 As String = $"HKEY_CURRENT_USER\Software\Classes\{protocolName}"
            Try
                Registry.CurrentUser.DeleteSubKeyTree(keyPath0, False)
                Return True
            Catch ex As Exception
                MsgBox(ex.ToString)
                Return False
            End Try
        End If

        Dim keyPath As String = $"HKEY_CURRENT_USER\Software\Classes\{protocolName}\shell\open\command"
        Dim keyValue As String = $"{applicationPath} ""%1"""

        Try
            ' Set the default value for the protocol's command
            Registry.SetValue(keyPath, "", keyValue, RegistryValueKind.String)

            ' Set the default value for the protocol itself
            keyPath = $"HKEY_CURRENT_USER\Software\Classes\{protocolName}"
            Registry.SetValue(keyPath, "", description, RegistryValueKind.String)

            ' Set the "URL Protocol" value
            Registry.SetValue(keyPath, "URL Protocol", "", RegistryValueKind.String)
            My.Settings.CustomGDPS = True
            My.Settings.Save()
            Return True
        Catch ex As Exception
            MsgBox(ex.ToString)
            Return False
        End Try

    End Function


End Module