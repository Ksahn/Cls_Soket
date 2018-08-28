Imports System.Net.Sockets
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Threading.Thread
Imports System.Threading
Imports System



Public Class Class1

#Region "Initial"
    Dim ipHostInfo As IPHostEntry = Dns.Resolve(Dns.GetHostName())
    Dim ipAddress As IPAddress = ipHostInfo.AddressList(0)
    Dim Socket_Server As New TcpListener(ipAddress, Socket_Ports)
    Dim Socket_Ports As Int16
    Dim Socket_ServerIP As String
    Dim Socket_Client As New TcpClient
    Dim Socket_Stram As NetworkStream
    Dim Socket_Server_Listening As TcpListener
    'Dim Socket_Stream_Out As Byte()
    Dim Socket_Stream_In(1024) As Byte

    Dim Rcv_data As String
    Public queue_data_pool As New Queue

    Public Thd_Rcv As New Threading.Thread(AddressOf Rcv)
#End Region
#Region "Stream"
    Public Sub Send(ByRef Socket_Stream_Out As Byte())
        Socket_Stram = Socket_Client.GetStream
        Socket_Stram.Write(Socket_Stream_Out, 0, Socket_Stream_Out.Length)
        Socket_Stram.Flush()
    End Sub
    Public Sub Rcv()
        Try
            If Not Socket_Client Is Nothing Then
                If Socket_Client.Connected = True Then
                    Socket_Stram = Socket_Client.GetStream
                    Socket_Client.ReceiveBufferSize = 1024
                    If Not Socket_Stram Is Nothing Then
                        If Socket_Stram.DataAvailable = True Then
                            While Socket_Stram.DataAvailable = True
                                Rcv_data = Socket_Stram.Read(Socket_Stream_In, 0, Socket_Stream_In.Length)
                                queue_data_pool.Enqueue(Rcv_data)
                            End While
                        End If
                    End If
                End If
            End If
        Catch
        End Try
    End Sub
#End Region
#Region "Connection"
    Private Sub Start_Server()
        Socket_Server.Start()
        While (True)
            Socket_Server.AcceptTcpClient()
        End While
        Socket_Server.AcceptTcpClient.Close()
        Socket_Server.Stop()
    End Sub
 
#End Region
#Region "Thread"

    Private Sub Thd_Check()
        Try
            If Not Thd_Rcv Is Nothing Then
                If Thd_Rcv.IsAlive = False Then
                    Thd_Rcv.Abort() : Thd_Rcv = Nothing : Thd_Rcv = New Thread(AddressOf Rcv) : Thd_Rcv.Name = "Thd_Rcv" : Thd_Rcv.Start()
                End If
            Else
                Thd_Rcv = New Thread(AddressOf Rcv) : Thd_Rcv.Name = "Thd_Rcv"
            End If
        Catch ex As Exception

        End Try
    End Sub
#End Region
End Class