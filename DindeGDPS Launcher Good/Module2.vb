Imports System.IO
Imports System.IO.Pipes
Imports System.Threading.Tasks
Module Module2
    Public Class ProClient
        Private Const PIPE_NAME As String = "LeWokisme"

        Public Sub SendMessage(message As String)
            Try
                Using client As New NamedPipeClientStream(".", PIPE_NAME, PipeDirection.InOut)
                    client.Connect()

                    Using writer As New StreamWriter(client)
                        writer.WriteLine(message)
                        writer.Flush()

                        ' Read response from Proc1
                        Using reader As New StreamReader(client)
                            Dim response As String = reader.ReadLine()
                            Console.WriteLine("Received response from Proc1: " & response)
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                Console.WriteLine("Error in ProClient: " & ex.Message)
            End Try
        End Sub
    End Class

    Public Class ProServer
        Private Const PIPE_NAME As String = "LeWokisme"

        Public Sub StartServer()
            Try
                Using server As New NamedPipeServerStream(PIPE_NAME, PipeDirection.InOut)
                    Console.WriteLine("Server waiting for connection...")
                    server.WaitForConnection()

                    Using reader As New StreamReader(server)
                        Dim message As String = reader.ReadLine()
                        Console.WriteLine("Received message from Proc2: " & message)

                        ' Process the message
                        ProcessMessage(message)

                        ' Respond to Proc2 if necessary
                        Dim response As String = "Message processed"
                        Using writer As New StreamWriter(server)
                            writer.WriteLine(response)
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                Console.WriteLine("Error in ProServer: " & ex.Message)
            End Try
        End Sub

        Private Async Sub ProcessMessage(message As String)
            ' Process the received message (e.g., close, restart)
            Select Case message
                Case "die"
                    ' Close the application
                    Application.Exit()
                Case "what"
                    ' Restart the application
                    While IsOnlyInstance() = False
                        Await Task.Delay(500)
                    End While
                    Application.Restart()
            End Select
        End Sub
    End Class
End Module
