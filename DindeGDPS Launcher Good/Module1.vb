Imports System.IO
Imports System.Net
Imports System.Threading
Imports System.Xml
Imports Microsoft.Win32
Imports System.Threading.Tasks
Imports System.Globalization
Imports Newtonsoft.Json
Imports System.Windows.Interop
Imports Newtonsoft.Json.Linq
Imports Newtonsoft
Imports System.Security.Policy
Imports Ionic.Zip
Imports System.Text
Public Module Module1

    Public RootFS = AppDomain.CurrentDomain.BaseDirectory
    Public nl = Environment.NewLine
    Public Origine = If(My.Settings.Language = "Default", SafeGuardLang(CultureInfo.CurrentCulture.TwoLetterISOLanguageName), SafeGuardLang(My.Settings.Language))

    ' Special settings [START]
    Public KernelExtender As Boolean = File.Exists(Path.Combine(RootFS, "conf", "kernelExt.txt"))
    Public LegacyNotifications As Boolean = File.Exists(Path.Combine(RootFS, "conf", "legacyNotify.txt"))
    ' Special settings [END]

    Public NotifyState = "Nothing" ' whatever lol

    Public KonamiNumber As Integer = 1
    Function Konami(key As Keys) As Boolean
        Select Case KonamiNumber
            Case 1, 2
                If key = Keys.Up Then
                    KonamiNumber += 1
                    Return True
                Else
                    KonamiNumber = 1
                    Return False
                End If
            Case 3, 4
                If key = Keys.Down Then
                    KonamiNumber += 1
                    Return True
                Else
                    KonamiNumber = 1
                    Return False
                End If
            Case 5, 7
                If key = Keys.Left Then
                    KonamiNumber += 1
                    Return True
                Else
                    KonamiNumber = 1
                    Return False
                End If
            Case 6, 8
                If key = Keys.Right Then
                    KonamiNumber += 1
                    Return True
                Else
                    KonamiNumber = 1
                    Return False
                End If
            Case 9
                If key = Keys.B Then
                    KonamiNumber += 1
                    Return True
                Else
                    KonamiNumber = 1
                    Return False
                End If
            Case 10
                If key = Keys.A Then
                    MsgBox("Konami code activated!", vbOKOnly, "Real")
                    MsgBox("INITIALIZING BANKAI", vbOKOnly, "Real")
                    ' Possibilities : none because I am a fucking dumbass
                    ' Now, everything is on virustotal
                    Process.Start("https://cowolon.pages.dev/")
                    KonamiNumber = 1
                    MsgBox("Hold up")
                    MsgBox("WHY THE HECK IS THIS SHIMMY SHIMMY YAY")
                    MsgBox("I THOUGHT IT WOULD BE GDCOLON ART")
                    MsgBox("NGAAAAAAAAAAAAAAAAAAAAAAAAAAAH")
                    Return True
                Else
                    KonamiNumber = 1
                    Return False
                End If
            Case Else
                KonamiNumber = 1
                Return False
        End Select
    End Function

    Sub Notify(Title As String, Description As String, NotifyStateString As String)
        If LegacyNotifications Then
            MsgBox(Description, vbOKOnly + vbInformation, Title)
            If NotifyStateString = "IM" Then
                Form4.Show()
            End If
        Else
            NotifyState = NotifyStateString
            Loader.NotifyIcon1.BalloonTipText = Description
            Loader.NotifyIcon1.BalloonTipTitle = Title
            Loader.NotifyIcon1.ShowBalloonTip(5000)
        End If
    End Sub
    Public Sub Play(PS As String, code As Keys) ' le parti socialiste
        My.Settings.DfPS = PS
        My.Settings.Save()
        Dim official As String() = {"gd19", "gd20", "gd21", "gd22", "gd22-old"}
        If Not official.Any(Function(value) PS = value) And My.Settings.IAmNotStupid = False Then
            Dim n = MsgBox("This is a custom, modded version of DindeGDPS (or another GDPS) and thus not by DimisAIO. Do you want to run this version? This message will not show up again once you click ""Yes""", vbYesNo + vbExclamation, "Modded version detected")
            If n = vbNo Then
                Return
            Else
                My.Settings.IAmNotStupid = True
                My.Settings.Save()
            End If
        ElseIf official.Any(Function(value) PS = value) AndAlso Not String.IsNullOrEmpty(My.Settings.token) Then
            BanCheck.ShowDialog()
        End If

        ' FixSave()  excuse me, why is this still there

        Dim gdpsFolderPath As String = Path.Combine(RootFS, PS)

        ' Since we have multiple DindeGDPS instances, we rely on info.xml, which means a whole writeup and multiple headaches

        ' Can't believe I used this in older versions. What if the admin disabled CMD?
        ' Shell("cmd.exe /c cd gdps & start DindeGDPS.exe & exit")

        Dim gdp = Path.Combine(RootFS, PS)
        Dim xmlDoc As New XmlDocument()
        xmlDoc.Load(Path.Combine(gdp, "info.xml"))
        Dim configElement As XmlElement = xmlDoc.DocumentElement

        Dim startupNode As XmlNode = configElement.SelectSingleNode("startup")
        Dim SM = ""
        Dim Principal = ""
        If code = Keys.Control Then
            Dim List = ""
            Dim files() As String
            files = Directory.GetFiles(gdp, "*.exe", SearchOption.TopDirectoryOnly)
            For Each FileName As String In files
                If Not FileName.ToLower.StartsWith("geode") Then
                    ' gtfo gayode
                    ' PTN POURQUOI IL FAUT QUE FILENAME RACCOURCI SOIT FRONT NATIONAL LOL
                    Dim FN = Path.GetFileName(FileName.Replace(".exe", ""))
                    If Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FN)) Then
                        If Not FN.Contains("-") Then
                            Principal = FN
                        Else
                            List += nl + FN
                        End If
                    End If
                End If
            Next
            SM = InputBox("Enter your session name. If it doesn't exist, it will be created." + List.Replace($"{Principal}-", ""))
            If Not String.IsNullOrEmpty(SM) AndAlso Not File.Exists(Path.Combine(gdp, $"{Principal}-{SM}.exe")) Then
                File.Copy(Path.Combine(gdp, $"{Principal}.exe"), Path.Combine(gdp, $"{Principal}-{SM}.exe"))
            End If
        End If
        If startupNode IsNot Nothing AndAlso startupNode.HasChildNodes Then
            For Each fileNode As XmlNode In startupNode.ChildNodes
                If fileNode.Name = "file" Then
                    Dim fileName As String = fileNode.InnerText
                    Dim startInfo As New ProcessStartInfo()

                    If fileName = "GeometryDash.exe" Then
                        ' Special handling!
                        Dim Source = Path.Combine(RootFS, "gd22", "Dinde.exe")
                        Dim Target = Path.Combine(RootFS, "gd22", "GeometryDash.exe")
                        gdp &= "22"
                        If File.Exists(Path.Combine(RootFS, "gd", "steam.txt")) Then
                            Process.Start("steam.exe steam://launch/322170")
                            Return
                        ElseIf Not File.Exists(Target) Then
                            If Not File.Exists(Source) Then
                                MsgBox("In order to open Geometry Dash via DindeGDPS, you must download DindeGDPS 2.2 first!", vbOKOnly + vbCritical, "DindeGDPS 2.2 not installed!")
                                Return
                            End If


                            Dim oldBytes As Byte() = Encoding.ASCII.GetBytes("https://gdps.dimisaio.be/")
                            Dim newBytes As Byte() = Encoding.ASCII.GetBytes("https://www.boomlings.com")

                            Dim data As Byte() = File.ReadAllBytes(Source)

                            Dim replacedCount As Integer = 0
                            For i As Integer = 0 To data.Length - oldBytes.Length
                                Dim match As Boolean = True
                                For j As Integer = 0 To oldBytes.Length - 1
                                    If data(i + j) <> oldBytes(j) Then
                                        match = False
                                        Exit For
                                    End If
                                Next
                                If match Then
                                    ' copy new bytes into place
                                    Array.Copy(newBytes, 0, data, i, newBytes.Length)
                                    replacedCount += 1
                                    ' advance i so overlapping matches are not re-detected
                                    i += oldBytes.Length - 1
                                End If
                            Next

                            If replacedCount = 0 Then
                                Throw New Exception("Original text not found in the executable.")
                            End If

                            File.WriteAllBytes(Target, data)
                        End If
                    End If

                    If Not String.IsNullOrEmpty(SM) AndAlso Principal = fileName.Replace(".exe", "") Then
                        startInfo.FileName = Path.Combine(gdp, $"{fileName.Replace(".exe", "")}-{SM}.exe")
                    Else
                        startInfo.FileName = Path.Combine(gdp, fileName)
                    End If

                    startInfo.WorkingDirectory = gdp

                    Dim proc As New Process()

                    proc.StartInfo = startInfo
                    proc.Start()

                    Thread.Sleep(250) ' condition de ta race
                End If
            Next
        End If

        If My.Settings.CloseLauncher Then
            Application.Exit()
        End If
    End Sub
    Public Sub ApplyColor(NColor As String)
        ChangeControlColors(Form4.Controls, NColor)
        ChangeControlColors(Form3.Controls, NColor)
        ChangeControlColors(BanCheck.Controls, NColor)
        ChangeControlColors(AboutView.Controls, NColor)
        ChangeControlColors(Form5.Controls, NColor)
        ChangeControlColors(Form6.Controls, NColor)

        If My.Settings.Simple = True Then
            ChangeControlColors(Simple.Controls, NColor)
        Else
            ChangeControlColors(Form1.Controls, NColor)
            ChangeMenuStripColors(Form1.Controls, NColor)
        End If
    End Sub
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

    Public Function SafeGuardLang(Langue As String)
        Dim languages = My.Computer.FileSystem.ReadAllText(Path.Combine(RootFS, "web", "installed_langs.txt")).Split("/")
        For Each language In languages
            If language = Langue Then
                Return Langue
            End If
        Next
        Return "en"
    End Function

    ' wtf am i doing
    Private listenerTask As Task

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

        If lver = "noupdate" Then
            Return True
        End If

        Dim updateUrlNode As XmlNode = config.SelectSingleNode("updateurl")
        Dim updateurl As String = If(updateUrlNode IsNot Nothing AndAlso Not String.IsNullOrEmpty(updateUrlNode.InnerText) AndAlso updateUrlNode.InnerText <> "default",
                                   updateUrlNode.InnerText,
                                   $"https://dogcheck.dimisaio.be/?client=game&gv={gdps}")


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
        Dim offcount = 0
        Dim offtitle = "GDPS"

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
                ' MsgBox(gdpsname)
                Dim test = Await GDPSVerCheck(gdpsname, False)

                If test = False Then
                    Dim o = True
                    Dim gdpstitle As String
                    Select Case gdpsname
                        Case "gd19"
                            gdpstitle = "DindeGDPS 1.9"
                        Case "gd20"
                            gdpstitle = "DindeGDPS 2.0"
                        Case "gd21"
                            gdpstitle = "DindeGDPS 2.1"
                        Case "gd22"
                            gdpstitle = "DindeGDPS 2.2"
                        Case Else
                            gdpstitle = gdpsname
                            o = False
                    End Select

                    If o Then
                        offcount += 1
                        offtitle = gdpstitle
                    Else
                        Notify("New update available!", gdpstitle & " got a new update! Go to the GDPS' download page or ask the owner for a link of the update!", "Nothing")
                    End If
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

        If offcount = 1 Then
            Notify("New update available!", offtitle & " got a new update! Click to open the Instance Manager!", "IM")
        ElseIf offcount > 1 Then
            Notify("New update available!", "Several DindeGDPS versions got updated! Click to open the Instance Manager!", "IM")
        End If

        ' hold on: he doesn't have a gdps?
        If dog = False Then
            Form4.Show()
            MsgBox("No GDPS instances have been detected! You can download one by clicking on ""Install""", vbOKOnly + vbExclamation, "No instances found")
        End If
    End Function

    Public Function MiniRefreshGDPS()
        ' Get an array of directory names that start with "gd"
        Dim directoryArray As String() = GetDirKeyword(RootFS)
        If Not My.Settings.Simple Then
            For Each gdps In directoryArray
                If Not File.Exists(Path.Combine(RootFS, gdps, "logo.png")) Then
                    File.Copy(Path.Combine(RootFS, "default.png"), Path.Combine(RootFS, gdps, "logo.png"))
                End If
            Next
        End If
    End Function
    Public Function RefreshGDPS()
        ' Get an array of directory names that start with "gd"
        Dim directoryArray As String() = GetDirKeyword(RootFS)

        Dim test As String = ""
        If Not File.Exists(Path.Combine(RootFS, "web", "list.js")) And Not My.Settings.Simple And directoryArray.Length > 0 Then
            For Each gdps In directoryArray
                test += gdps & "||"
                If Not File.Exists(Path.Combine(RootFS, gdps, "logo.png")) Then
                    File.Copy(Path.Combine(RootFS, "default.png"), Path.Combine(RootFS, gdps, "logo.png"))
                End If
            Next
            test = test.Substring(0, test.Length - 2)
            test = $"const gdpses = `{test}`"
            File.WriteAllText(Path.Combine(RootFS, "web", "list.js"), test)
        End If

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
            Dim keyPath0 As String = $"Software\Classes\{protocolName}"
            Try
                Registry.CurrentUser.DeleteSubKeyTree(keyPath0, True)
                My.Settings.CustomGDPS = False
                My.Settings.Save()
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

    Public Sub Backup(ver As String)
        If Directory.Exists(Path.Combine(RootFS, ver)) Then
            ' Create a backup directory :)
            Directory.CreateDirectory(Path.Combine(RootFS, ver + "-backup"))
            ' It is primordial to copy the menuloop over
            If File.Exists(Path.Combine(RootFS, ver, "Resources", "menuLoop.mp3")) Then
                Directory.CreateDirectory(Path.Combine(RootFS, ver + "-backup", "Resources"))
                File.Copy(Path.Combine(RootFS, ver, "Resources", "menuLoop.mp3"), Path.Combine(RootFS, ver + "-backup", "Resources", "menuLoop.mp3"))
            End If
            If File.Exists(Path.Combine(RootFS, ver, "keep.txt")) Then
                File.Copy(Path.Combine(RootFS, ver, "keep.txt"), Path.Combine(RootFS, ver + "-backup", "keep.txt"))
                Dim x = File.ReadAllText(Path.Combine(RootFS, ver, "keep.txt"))
                Dim x2 = x.Split({vbCrLf, vbCr, vbLf}, StringSplitOptions.None)
                For Each xw As String In x2
                    Select Case True
                        Case File.Exists(Path.Combine(RootFS, ver, xw))
                            If Not Directory.Exists(Directory.GetParent(xw).ToString) Then
                                Directory.CreateDirectory(Path.Combine(RootFS, ver + "-backup", Path.GetDirectoryName(xw)))
                            End If
                            File.Copy(Path.Combine(RootFS, ver, xw), Path.Combine(RootFS, ver + "-backup", xw))
                        Case Directory.Exists(Path.Combine(RootFS, ver, xw))
                            If Not Directory.Exists(Directory.GetParent(xw).ToString) Then
                                Directory.CreateDirectory(Path.Combine(RootFS, ver + "-backup", Path.GetDirectoryName(xw)))
                            End If
                            My.Computer.FileSystem.CopyDirectory(Path.Combine(RootFS, ver, xw), Path.Combine(RootFS, ver + "-backup", xw))
                    End Select
                Next
            End If
            Directory.Delete(Path.Combine(RootFS, ver), True)
        End If
    End Sub
    Public Sub Restore(ver As String)
        If File.Exists(Path.Combine(RootFS, ver + "-backup", "Resources", "menuLoop.mp3")) Then
            File.Delete(Path.Combine(RootFS, ver, "Resources", "menuLoop.mp3"))
            File.Move(Path.Combine(RootFS, ver + "-backup", "Resources", "menuLoop.mp3"), Path.Combine(RootFS, ver, "Resources", "menuLoop.mp3"))
        End If
        If File.Exists(Path.Combine(RootFS, ver + "-backup", "keep.txt")) Then
            File.Copy(Path.Combine(RootFS, ver + "-backup", "keep.txt"), Path.Combine(RootFS, ver, "keep.txt"), True)
            Dim x = File.ReadAllText(Path.Combine(RootFS, ver + "-backup", "keep.txt"))
            Dim x2 = x.Split({vbCrLf, vbCr, vbLf}, StringSplitOptions.None)
            For Each xw As String In x2
                Select Case True
                    Case File.Exists(Path.Combine(RootFS, ver + "-backup", xw))
                        If Not Directory.Exists(Directory.GetParent(xw).ToString) Then
                            Directory.CreateDirectory(Path.Combine(RootFS, ver, Path.GetDirectoryName(xw)))
                        End If
                        File.Copy(Path.Combine(RootFS, ver + "-backup", xw), Path.Combine(RootFS, ver, xw))
                    Case Directory.Exists(Path.Combine(RootFS, ver + "-backup", xw))
                        If Not Directory.Exists(Directory.GetParent(xw).ToString) Then
                            Directory.CreateDirectory(Path.Combine(RootFS, ver, Path.GetDirectoryName(xw)))
                        End If
                        My.Computer.FileSystem.CopyDirectory(Path.Combine(RootFS, ver + "-backup", xw), Path.Combine(RootFS, ver, xw))
                End Select
            Next
        End If
        Directory.Delete(Path.Combine(RootFS, ver + "-backup"), True)
    End Sub

    Public Function ReadFlag(FlagName As String)
        Dim BasePath = Path.Combine(RootFS, "conf", $"{FlagName}.txt")
        If File.Exists(BasePath) Then
            Dim FileReader = File.ReadAllText(BasePath)
            MsgBox(FileReader.Length)
            If FileReader.Length > 0 Then
                Return FileReader
            Else
                Return True
            End If
        End If
        Return False
    End Function

    Public Async Sub CheckForUpdates()
        If My.Settings.DisableUpd = True Then Return
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls11
        Dim wc As New WebClient()
        wc.Headers.Add("User-Agent", "DindeGDPS/" & Application.ProductVersion)

        Dim url As String = "https://api.github.com/repos/DimisAIO/DindeGDPS-Launcher/releases"
        If My.Settings.Channel <> "Beta" Then
            url &= "/latest"
        Else
            url &= "?per_page=5"
        End If

        Dim UpdateListReq As String
        Dim UpdateList As List(Of Release)
        Try
            UpdateListReq = Await wc.DownloadStringTaskAsync(url)
            If My.Settings.Channel <> "Beta" Then UpdateListReq = "[" & UpdateListReq & "]" ' hmm

            ' Deserialize into a list of releases
            UpdateList = JsonConvert.DeserializeObject(Of List(Of Release))(UpdateListReq)
        Catch ex As Exception
            Console.WriteLine("OFFLINE!!!")
            Return
        End Try

        ' Loop through each release
        For Each Update In UpdateList
            If My.Settings.Channel = "Beta" AndAlso Update.PreRelease = False Then
                Continue For
            End If

            If Application.ProductVersion = Update.Version Then
                Continue For
            End If

            Dim DownloadLink = Update.Assets?.FirstOrDefault().BrowserDownloadUrl
            If String.IsNullOrEmpty(DownloadLink) Then
                CheckForWebUpdates()
                Return
            Else
                UpdateUrl = DownloadLink
                If My.Settings.Simple = True Then
                    Simple.LinkLabel1.Visible = True
                Else
                    Form1.LinkLabel2.Visible = True
                End If
                Return
            End If
        Next
        CheckForWebUpdates()
    End Sub

    Public Async Sub CheckForWebUpdates()
        If My.Settings.DisableUpd Or My.Settings.Simple Then Return
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls11
        Dim wc As New WebClient()
        wc.Headers.Add("User-Agent", "DindeGDPS/" & Application.ProductVersion)

        Dim url As String = "https://api.github.com/repos/DimisAIO/DindeGDPS-Launcher-web/releases/latest"

        Dim UpdateListReq As String = Await wc.DownloadStringTaskAsync(url)
        UpdateListReq = "[" & UpdateListReq & "]" ' hmm

        ' Deserialize into a list of releases
        Dim UpdateList As List(Of Release) = JsonConvert.DeserializeObject(Of List(Of Release))(UpdateListReq)

        ' Loop through each release
        For Each Update In UpdateList

            If My.Settings.WebVersion = Update.Version Then
                Return
            End If

            Dim DownloadLink = Update.Assets?.FirstOrDefault().BrowserDownloadUrl
            If String.IsNullOrEmpty(DownloadLink) Then
                Return
            Else
                Form1.LinkLabel3.Visible = True
                NextWebVersion = Update.Version
            End If
        Next
    End Sub

    Private UpdateUrl As String
    Private NextWebVersion As String
    Public Async Sub UpdateLauncher()
        If String.IsNullOrEmpty(UpdateUrl) Then Return
        If File.Exists(Path.Combine(RootFS, "setup.exe")) Then
            File.Delete(Path.Combine(RootFS, "setup.exe"))
        End If
        Dim wc As New WebClient
        Await wc.DownloadFileTaskAsync(New Uri(UpdateUrl), Path.Combine(RootFS, "setup.exe"))
        Dim Setup As New ProcessStartInfo()
        Setup.FileName = Path.Combine(RootFS, "setup.exe")
        Setup.Arguments = "/passive"
        Dim Exec As New Process()
        Exec.StartInfo = Setup
        Exec.Start()
        Application.Exit()
    End Sub

    Public Async Sub UpdateWeb()
        If File.Exists(Path.Combine(RootFS, "web.zip")) Then
            File.Delete(Path.Combine(RootFS, "web.zip"))
        End If
        Dim wc As New WebClient
        ' fml why did I put cdn-dinde behind cloudflare
        ' Randomize()
        ' Await wc.DownloadFileTaskAsync(New Uri("https://cdn-dinde.141412.xyz/web.zip?" + CInt(Int((99999 * Rnd()) + 1)).ToString), Path.Combine(RootFS, "web.zip"))
        Await wc.DownloadFileTaskAsync(New Uri("https://github.com/DimisAIO/DindeGDPS-Launcher-web/releases/latest/download/web.zip"), Path.Combine(RootFS, "web.zip"))
        If Directory.Exists(Path.Combine(RootFS, "web")) Then
            Directory.Delete(Path.Combine(RootFS, "web"), True)
        End If
        Directory.CreateDirectory(Path.Combine(RootFS, "web"))
        Dim extract = Task.Run(Sub()
                                   Using zip As ZipFile = ZipFile.Read("web.zip")
                                       For Each entry As ZipEntry In zip
                                           entry.Extract("web", ExtractExistingFileAction.OverwriteSilently)
                                       Next
                                   End Using
                                   File.Delete("web.zip")
                               End Sub)
        Await extract
        My.Settings.WebVersion = NextWebVersion
        NextWebVersion = ""
        My.Settings.Save()
        Form1.GoHome()
        Form1.LinkLabel3.Hide()
    End Sub
End Module

Public Class Release
    <JsonProperty("tag_name")>
    Public Property Version As String

    <JsonProperty("prerelease")>
    Public Property PreRelease As Boolean

    <JsonProperty("assets")>
    Public Property Assets As List(Of Asset)
End Class

Public Class Asset
    <JsonProperty("browser_download_url")>
    Public Property BrowserDownloadUrl As String
End Class
