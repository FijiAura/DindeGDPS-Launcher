Imports System.IO
Imports System.Xml
Imports Ionic.Zip
Imports System.Threading.Tasks
Imports System.Net
Imports System.Runtime.ConstrainedExecution
Public Class Form5
    Public Success = False
    Public url As String
    Public psname As String
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Public Function Fork(arg As String)
        ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)
        ' The first argument is the executable path, so we start from the second argument

        Dim url As String = arg.Replace("dgdps://", "https://").TrimEnd("/")
        ' ugh actually dgdps:// is actually https://, it's just that dindegdps is not a browser, right? lol imagine

        ' Download and display the logo image
        Try
            Dim dl As New WebClient
            Dim imageBytes As Byte() = dl.DownloadData(url + "/logo.png")
            Using ms As New MemoryStream(imageBytes)
                ' Set the PictureBox's image from the MemoryStream
                PictureBox1.Image = Image.FromStream(ms)
            End Using
        Catch ex As Exception
            MsgBox($"Error downloading logo image: {ex.Message}")
            Return False
        End Try

        ' Download and display XML content
        Dim xml As New XmlDocument()
        Dim encodedUrl As String = Uri.EscapeUriString(url + "/install.xml")
        Dim xmlContent As String = DownloadXmlContent(encodedUrl)

        ' Check if the XML content was successfully downloaded
        If Not String.IsNullOrEmpty(xmlContent) Then
            xml.LoadXml(xmlContent)

            ' Get the root element of the XML document
            Dim config As XmlElement = xml.DocumentElement

            ' Check if the root element is not null
            If config IsNot Nothing Then
                ' Access the "game" node and update Label2.Text
                Dim gameNode As XmlNode = config.SelectSingleNode("game")
                Dim author As XmlNode = config.SelectSingleNode("author")
                If gameNode IsNot Nothing And author IsNot Nothing Then
                    psname = gameNode.InnerText
                    Dim official As String() = {"gd19", "gd20", "gd21", "gd22", "gd22-old"}
                    If official.Any(Function(value) psname = value) Then
                        MsgBox("The GDPS cannot use gd19, gd20, gd21, gd22-old or gd22 as name! Changing it will fix it", vbOKOnly + vbCritical, "DindeGDPS")
                        Return False
                    End If
                    Me.url = $"{url}/{gameNode.InnerText}.zip"
                    Label1.Text = Label1.Text.Replace("GDPS", psname)
                    Label2.Text = Label2.Text + $"{url}/{psname}.zip"
                    Label3.Text = Label3.Text + $"{author.InnerText}"
                    Return True
                Else
                    Console.WriteLine("Game node not found.")
                    Return False
                End If
            Else
                Console.WriteLine("Root element not found.")
                Return False
            End If
        Else
            Console.WriteLine("Failed to download XML content.")
            Return False
        End If
    End Function

    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' MsgBox(url)
        ' MsgBox(psname)
        ' enough debugging
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        PSInst(psname, url)
    End Sub

    Private Async Sub PSInst(name As String, url As String)
        Dim chk = False
        If Directory.Exists(Path.Combine(RootFS, name)) Then
            Backup(name)
            chk = True
        End If
        Label5.Text = "Downloading..."
        ' tous les memes
        Dim verzip = Path.Combine(RootFS, $"{name}.zip")
        name = Path.Combine(RootFS, name)
        Await DownloadAsync(url, verzip)
        Directory.CreateDirectory(name)

        Dim extract = Task.Run(Sub()
                                   Using zip As ZipFile = ZipFile.Read(verzip)
                                       For Each entry As ZipEntry In zip
                                           entry.Extract(name, ExtractExistingFileAction.OverwriteSilently)
                                           Me.Invoke(Sub()
                                                         Me.Label5.Text = "Extracting " + entry.FileName
                                                     End Sub)
                                       Next
                                   End Using
                                   File.Delete(verzip)
                               End Sub)
        Await extract

        If chk Then
            Restore(name)
        End If

        Label5.Text = "Done!"
        MsgBox("Installed successfully!" + nl + "DindeGDPS will (re)start.", vbOKOnly + vbInformation, "Instance Manager")

        Success = True

        Close()
    End Sub
    Private Sub DownloadProgressCallback(sender As Object, e As DownloadProgressChangedEventArgs)
        ' Calculate the progress percentage
        Dim progressPercentage As Integer = CInt((CDbl(e.BytesReceived) / e.TotalBytesToReceive) * 100)

        ' Print or use the progress information as needed
        Label5.Text = "Downloading (" & progressPercentage & "%)"
    End Sub
    Private Async Function DownloadFileAsync(wc As WebClient, uri As Uri, destinationPath As String) As Task
        Await wc.DownloadFileTaskAsync(uri, destinationPath)
    End Function
    Private Async Function DownloadAsync(aurl As String, dest As String) As Threading.Tasks.Task
        ServicePointManager.SecurityProtocol = DirectCast(3072, System.Net.SecurityProtocolType)
        Dim wc As New WebClient
        AddHandler wc.DownloadProgressChanged, AddressOf DownloadProgressCallback

        Try
            Await DownloadFileAsync(wc, New Uri(aurl), dest)
            Console.WriteLine("Download completed successfully.")
        Catch ex As Exception
            Console.WriteLine($"Error downloading file: {ex.Message}")
        End Try
        ' Await wc.DownloadFileTaskAsync(url, Path.Combine(rn, dest))
        Return
    End Function
End Class