Imports System.IO
Imports Newtonsoft.Json.Linq

Public Class Form6
    Function IsAlphaNum(ByVal StringToCheck As String)
        For i = 0 To StringToCheck.Length - 1
            If Char.IsLetter(StringToCheck.Chars(i)) Or Char.IsNumber(StringToCheck.Chars(i)) Then
                Return True
            End If
        Next
        Return False
    End Function
    Private Async Sub TextBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim CommandText = TextBox2.Text.ToLower()
            TextBox2.Text = ""
            Select Case CommandText
                Case "triggerbancheck"
                    BanCheck.ShowDialog()
                    Return
                Case "help"
                    ' TextBox1.Text += $"Commands:{nl}flag on: turns on a flag{nl}flag off: turns off flag{nl}Example: kernelExt on{nl}clear: Clears console{nl}help: shows help message{nl}triggerbancheck: triggers a Connecting to servers / Ban check{nl}"
                    ShowHelp() ' eπσιλον
                    Return
                Case "clear"
                    TextBox1.Text = ""
                    Return
            End Select
            Dim Command() = CommandText.Split(" "c)
            For Each CommandTest In Command
                If Command(0) = "geode" Then
                    CommandTest = CommandTest.Replace(".", "").Replace("@", "")
                End If
                If Not IsAlphaNum(CommandTest) Then Return
            Next
            Select Case Command(0)
                Case "load"
                    TextBox1.Text &= "Loading " & Command(1) & nl
                    Play(Command(1), Nothing)
                Case "readflag"
                    If Command.Length < 2 Then
                        TextBox1.Text += "readflag flag1 flag2..." + nl
                        Return
                    End If
                    For Each FlagItem As String In Command.Skip(1)
                        Dim FlagReader = ReadFlag(FlagItem)
                        TextBox1.Text += FlagItem + ": " + FlagReader.ToString() + nl
                    Next
                Case "version"
                    If Command.Length < 3 Then
                        TextBox1.Text += "version [install / uninstall / update] gd21 gd22..." + nl
                        Return
                    End If
                    For Each InstItem As String In Command.Skip(2)
                        TextBox1.Text += "Processing " + InstItem + nl
                        Dim Correct = {"gd19", "gd20", "gd21", "gd22"}
                        If Not Correct.Contains(InstItem) Then
                            TextBox1.Text += InstItem + " is not a valid version (gd19, gd20...)." + nl
                            Continue For
                        End If
                        Select Case Command(1)
                            Case "uninstall"
                                If Directory.Exists(Path.Combine(RootFS, InstItem, "Resources")) Then
                                    Directory.Delete(Path.Combine(RootFS, InstItem), True)
                                End If
                            Case "install", "update"
                                Await Form4.PSInst(InstItem, True)
                        End Select
                        TextBox1.Text += InstItem + " has been processed successfully" + nl
                    Next
                Case "geode"
                    If Command.Length < 3 Then
                        TextBox1.Text += "geode [install / uninstall] dankmeme.globed2 qolmod ..." + nl + "Only works on DindeGDPS 2.2" + nl
                        Return
                    End If
                    Dim Geode22File = Path.Combine(RootFS, "gd22", "Geode.dll")
                    If Not File.Exists(Geode22File) Then
                        TextBox1.Text += "DindeGDPS 2.2 must be installed with Geode first!" + nl
                        Return
                    End If
                    Select Case Command(1)
                        Case "install"
                            Dim GeodeVer As String = FileVersionInfo.GetVersionInfo(Geode22File).FileVersion.Replace(",", ".")
                            GeodeVer = GeodeVer.Substring(0, GeodeVer.Length - 2)
                            TextBox1.Text += "Using Geode version " + GeodeVer + nl
                            InstallGeodeMods(Command.Skip(2), GeodeVer)
                        Case "uninstall"
                            For Each GeodeMod As String In Command.Skip(2)
                                Dim UnPath = Path.Combine(RootFS, "gd22", "geode", "mods", GeodeMod & ".geode")
                                Dim UnPathDir = Path.Combine(RootFS, "gd22", "geode", "unzipped", GeodeMod)
                                If File.Exists(UnPath) Then File.Delete(UnPath)
                                If Directory.Exists(UnPathDir) Then Directory.Delete(UnPathDir, True)
                                TextBox1.Text += "Successfully uninstalled " + GeodeMod + nl
                            Next
                            TextBox1.Text += "WARNING: THE GEODE COMMAND DOESN'T HANDLE DEPENDENCIES." + nl + "If a mod depended on a mod you uninstalled, please install it again!"
                    End Select
                Case Else
                    If Command.Length < 2 Then
                        TextBox1.Text += "Error, please run the help command for information." + nl
                        Return
                    End If
                    Dim FilePath = Path.Combine(RootFS, "conf", $"{Command(0)}.txt")
                    Select Case Command(1)
                        Case "off", "false"
                            If File.Exists(FilePath) Then
                                File.Delete(FilePath)
                            End If
                        Case Else
                            File.WriteAllText(FilePath, Command(1))
                    End Select
                    TextBox1.Text += $"Flag {Command(0)} set to {Command(1)}{nl}"
            End Select
        End If
    End Sub

    Private Sub InstallGeodeMods(GeodeMods, GeodeVer)
        Dim WC As New Net.WebClient
        For Each GeodeMod As String In GeodeMods
            Dim ModVersion = "latest"
            If GeodeMod.Contains("@") Then
                Dim ModString As String() = GeodeMod.Split("@")
                ModVersion = ModString(1)
                GeodeMod = ModString(0)
            End If
            If String.IsNullOrEmpty(ModVersion) Then
                TextBox1.Text += "No version has been specified for " + GeodeMod + nl
                Return
            End If
            TextBox1.Text += "Processing " + GeodeMod + " version " + ModVersion + nl
            Dim SearchRequest As String = WC.DownloadString("https://api.geode-sdk.org/v1/mods?geode=" + GeodeVer + "&query=" + GeodeMod)
            Dim SearchResult As JObject = JObject.Parse(SearchRequest)
            Dim ModID As String = SearchResult.SelectToken("payload.data[0].id")
            If String.IsNullOrEmpty(ModID) Then
                TextBox1.Text += "Cannot find " + GeodeMod + nl + "(did you update geode?)" + nl
                Return
            End If
            Dim ModRequest = WC.DownloadString("https://api.geode-sdk.org/v1/mods/" + ModID + "/versions/" + ModVersion)
            Dim ModResult As JObject = JObject.Parse(ModRequest)
            Dim DownloadLink As String = ModResult.SelectToken("payload.download_link")
            If String.IsNullOrEmpty(DownloadLink) Then
                TextBox1.Text += "Cannot find version " + ModVersion + " for " + GeodeMod + nl + "(try latest?)" + nl
                Return
            End If
            TextBox1.Text += "Downloading " + ModID + " version " + ModVersion + nl
            Dim ModDependencies = ModResult.SelectTokens("payload.dependencies[*].mod_id")
            Dim NextMods As New List(Of String)
            For Each ModDependency In ModDependencies
                NextMods.Add(ModDependency.ToString())
            Next

            Dim FinalName = Path.Combine(RootFS, "gd22", "geode", "mods", ModID & ".geode")
            WC.DownloadFile(DownloadLink, FinalName)

            TextBox1.Text += "Successfully installed " + GeodeMod + nl

            If NextMods.Count > 0 Then
                InstallGeodeMods(NextMods, GeodeVer)
            End If
        Next
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        TextBox1.SelectionStart = TextBox1.Text.Length
        TextBox1.ScrollToCaret()
    End Sub
    Private Sub ShowHelp(Optional AlsoClearScren As Boolean = False)
        If File.Exists(Path.Combine(RootFS, "HiConsole.txt")) Then
            Dim Text As String = File.ReadAllText(Path.Combine(RootFS, "HiConsole.txt"))
            If AlsoClearScren Then
                TextBox1.Text = Text & nl
            Else
                TextBox1.Text &= Text & nl
            End If
        End If
    End Sub

    Private Sub Form6_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ShowHelp(True)
    End Sub
End Class