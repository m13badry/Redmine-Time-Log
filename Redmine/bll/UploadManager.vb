Imports System.Net
Imports System.IO
Imports System.Text

Public Class UploadManager

    Enum HttpMethod
        POST
        [GET]
    End Enum

#Region "Fields"

    Private _timeout As Integer
    Private _httpProtocolVersion As Version
    Private _keepAlive As Boolean
    Private _isBusy As Boolean
    Private _operationCanceled As Boolean
    Private _requestHeaders As WebHeaderCollection
    Private _encoding As System.Text.Encoding
#End Region

#Region "Delegates"

    'Event Handlers delegates
    Public Delegate Sub SendDataProgressChangedEventHandler(ByVal sender As Object, ByVal e As SendDataProgressChangedEventArgs)
    Public Delegate Sub SendDataCompletedEventHandler(ByVal sender As Object, ByVal e As SendDataCompletedEventArgs)

    'Async Operation Delegates
    Private _onProgressReportDelegate As System.Threading.SendOrPostCallback
    Private _onSendDataCompletedDelegate As System.Threading.SendOrPostCallback

    'Worker Delegates
    Private Delegate Sub WorkerEventHandler(ByVal url As String, ByVal method As String, ByVal postData As System.Collections.Specialized.NameValueCollection, ByVal postFiles As System.Collections.Specialized.NameValueCollection, ByVal asyncOp As System.ComponentModel.AsyncOperation)

#End Region

#Region "Events"

    Public Event SendDataProgressChanged As SendDataProgressChangedEventHandler
    Public Event SendDataCompleted As SendDataCompletedEventHandler

#End Region

#Region "Construction"

    Public Sub New()
        'Initialize fields with default values
        _timeout = 30000 '30 seconds
        'Most of the web servers support Http Protocol Version 1.1
        _httpProtocolVersion = HttpVersion.Version11
        _keepAlive = True
        _isBusy = False
        _operationCanceled = False
        'Load the default encoding UTF-8
        _encoding = New UTF8Encoding()
    End Sub

    Private Sub initializeDelegates()
        _onProgressReportDelegate = New System.Threading.SendOrPostCallback(AddressOf reportProgress)
        _onSendDataCompletedDelegate = New System.Threading.SendOrPostCallback(AddressOf sendCompleted)
    End Sub

#End Region

#Region "Asyncronous Operations"

    Public Sub SendDataAsync(ByVal url As String, ByVal postData As System.Collections.Specialized.NameValueCollection)
        SendDataAsync(url, "POST", postData, Nothing)
    End Sub

    Public Sub SendDataAsync(ByVal url As String, ByVal method As String, ByVal postData As System.Collections.Specialized.NameValueCollection)
        SendDataAsync(url, method, postData, Nothing)
    End Sub

    Public Sub SendDataAsync(ByVal url As String, ByVal method As String, ByVal postData As System.Collections.Specialized.NameValueCollection, ByVal postFiles As System.Collections.Specialized.NameValueCollection)
        'Check if UploadManager is busy
        If Me.IsBusy Then
            Throw New InvalidOperationException("Cannot invoke SendDataAsync while it is already being invoked.")
        End If

        If String.IsNullOrEmpty(url) Then
            Throw New ArgumentNullException("url", "url cannot be null or empty.")
        End If

        initializeDelegates()

        Dim asyncOp As System.ComponentModel.AsyncOperation = System.ComponentModel.AsyncOperationManager.CreateOperation(Nothing)
        Dim workerDelegate As New WorkerEventHandler(AddressOf sendDataWorker)

        workerDelegate.BeginInvoke(url, method, postData, postFiles, asyncOp, Nothing, Nothing)

    End Sub

    Private Sub sendDataWorker(ByVal url As String, ByVal method As String, ByVal postData As System.Collections.Specialized.NameValueCollection, ByVal postFiles As System.Collections.Specialized.NameValueCollection, ByVal asyncOp As System.ComponentModel.AsyncOperation)
        'Declare variables
        Dim request As HttpWebRequest = Nothing
        Dim response As HttpWebResponse = Nothing
        Dim responseReader As StreamReader = Nothing
        Dim localFileStream As FileStream = Nothing

        'Boundary header is used for post stream.
        Dim boundaryHeader As String = "f93dcbA3"
        Dim boundary As String = "--" & boundaryHeader
        Dim footer As String = vbCrLf + boundary + "--" + vbCrLf

        'Set OperationCanceled flag to false
        _operationCanceled = False

        'Build Header
        Dim headerBytes() As Byte
        Dim picFile As FileInfo
        Dim footerBytes() As Byte = _encoding.GetBytes(footer)

        Dim progressChanged As SendDataProgressChangedEventArgs
        Dim operationCompleted As SendDataCompletedEventArgs

        Dim bytesSent As Long = 0
        Dim totalBytesToSend As Long = 0
        Dim progressPercentage As Integer

        Dim curFileName As String = String.Empty

        Try
            request = CType(WebRequest.Create(url), HttpWebRequest)
            If String.IsNullOrEmpty(method) Then
                request.Method = "POST"
            Else
                request.Method = method
            End If

            request.ContentType = String.Format("multipart/form-data; boundary={0}", boundaryHeader)

            If _requestHeaders IsNot Nothing Then
                request.Headers = _requestHeaders
            End If
            'Calculate total bytes to be sent
            request.ContentLength = calculateTotalSize(boundary, footer, postData, postFiles)
            request.ReadWriteTimeout = _timeout
            request.KeepAlive = _keepAlive
            request.ProtocolVersion = _httpProtocolVersion
            'Calculate total bytes to be sent
            totalBytesToSend = request.ContentLength

            Dim requestStream As Stream = request.GetRequestStream()

            If postFiles IsNot Nothing Then
                For Each key As String In postFiles.AllKeys
                    'check if operation was canceled
                    If _operationCanceled Then
                        Exit For
                    End If

                    picFile = New FileInfo(postFiles(key))
                    curFileName = picFile.Name

                    'Write header
                    headerBytes = _encoding.GetBytes(buildFileHeader(boundary, key, postFiles.Get(key)))
                    requestStream.Write(headerBytes, 0, headerBytes.Length)

                    'Update progress
                    bytesSent += headerBytes.Length
                    progressPercentage = CInt((CSng(bytesSent) / CSng(totalBytesToSend)) * 100)
                    progressChanged = New SendDataProgressChangedEventArgs(progressPercentage, bytesSent, totalBytesToSend, curFileName)

                    'Invoke the reportProgress delegate on the right thread for the application
                    asyncOp.Post(AddressOf reportProgress, progressChanged)

                    'Open the file.
                    localFileStream = picFile.Open(FileMode.Open, FileAccess.Read)
                    Dim bufferSize As Integer = 2047
                    Dim buffer(bufferSize) As Byte
                    Dim bytesRead As Integer = 0

                    'Upload the file by writing the file content to the stream.
                    Do
                        'check if operation was canceled
                        If _operationCanceled Then
                            localFileStream.Close()
                            Exit For
                        End If

                        bytesRead = localFileStream.Read(buffer, 0, bufferSize)
                        requestStream.Write(buffer, 0, bytesRead)
                        'Update progress
                        bytesSent += bytesRead
                        progressPercentage = CInt((CSng(bytesSent) / CSng(totalBytesToSend)) * 100)
                        progressChanged = New SendDataProgressChangedEventArgs(progressPercentage, bytesSent, totalBytesToSend, curFileName)

                        'Invokes the delegate on the appropriate thread for the application.
                        asyncOp.Post(AddressOf reportProgress, progressChanged)
                    Loop While bytesRead > 0

                    localFileStream.Close()
                Next
            End If

            If postData IsNot Nothing Then
                For Each key As String In postData.AllKeys
                    'check if operation was canceled
                    If _operationCanceled Then
                        Exit For
                    End If

                    'Write header
                    curFileName = key
                    headerBytes = _encoding.GetBytes(buildDataHeader(boundary, key, postData.Get(key)))
                    requestStream.Write(headerBytes, 0, headerBytes.Length)

                    bytesSent += headerBytes.Length

                    'Update progress
                    progressPercentage = CInt((CSng(bytesSent) / CSng(totalBytesToSend)) * 100)
                    progressChanged = New SendDataProgressChangedEventArgs(progressPercentage, bytesSent, totalBytesToSend, curFileName)
                    asyncOp.Post(AddressOf reportProgress, progressChanged)
                Next
            End If

            'check if operation was canceled
            If _operationCanceled Then
                request.Abort()
                operationCompleted = New SendDataCompletedEventArgs(Nothing, True, Nothing)
            Else
                'Write footer
                requestStream.Write(footerBytes, 0, footerBytes.Length)
                requestStream.Close()

                bytesSent += footerBytes.Length

                'Update progress
                progressPercentage = CInt((CSng(bytesSent) / CSng(totalBytesToSend)) * 100)
                progressChanged = New SendDataProgressChangedEventArgs(progressPercentage, bytesSent, totalBytesToSend, curFileName)
                asyncOp.Post(AddressOf reportProgress, progressChanged)

                response = CType(request.GetResponse(), HttpWebResponse)
                responseReader = New StreamReader(response.GetResponseStream())
                operationCompleted = New SendDataCompletedEventArgs(Nothing, False, responseReader.ReadToEnd())
            End If
        Catch wex As WebException
            If wex.Status = WebExceptionStatus.RequestCanceled Then
                operationCompleted = New SendDataCompletedEventArgs(wex, True, Nothing)
            Else
                operationCompleted = New SendDataCompletedEventArgs(wex, False, Nothing)
            End If
        Catch ex As Exception
            operationCompleted = New SendDataCompletedEventArgs(ex, False, Nothing)
        Finally
            'Clean up resources
            If response IsNot Nothing Then
                response.Close()
            End If

            If responseReader IsNot Nothing Then
                responseReader.Close()
            End If

            If localFileStream IsNot Nothing Then
                localFileStream.Close()
            End If
        End Try

        asyncOp.PostOperationCompleted(_onSendDataCompletedDelegate, operationCompleted)
    End Sub

    Public Sub CancelAsync()
        _operationCanceled = True
    End Sub

#End Region

#Region "Helper Methods"

    'This method is guaranteed to be called on the correct thread
    Private Sub reportProgress(ByVal state As Object)
        Dim e As SendDataProgressChangedEventArgs = CType(state, SendDataProgressChangedEventArgs)
        OnSendDataProgressChanged(e)
    End Sub

    Private Sub sendCompleted(ByVal state As Object)
        Dim e As SendDataCompletedEventArgs = CType(state, SendDataCompletedEventArgs)
        OnSendDataCompleted(e)
    End Sub
#End Region

#Region "Events"

    Protected Sub OnSendDataProgressChanged(ByVal e As SendDataProgressChangedEventArgs)
        RaiseEvent SendDataProgressChanged(Me, e)
    End Sub

    Protected Sub OnSendDataCompleted(ByVal e As SendDataCompletedEventArgs)
        RaiseEvent SendDataCompleted(Me, e)
    End Sub

#End Region

#Region "Synchronous Operations"

    ''' <summary>
    ''' Uploads a string data to a specific resource
    ''' </summary>
    ''' <param name="url">The resource Url</param>
    ''' <param name="data">Data to be uploaded to the specified resource url.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UploadString(ByVal url As String, ByVal data As String) As String
        Return UploadString(url, "POST", data)
    End Function

    ''' <summary>
    ''' Uploads a string data to a specific resource
    ''' </summary>
    ''' <param name="url">The resource Url</param>
    ''' <param name="method">The request method. The default method is "POST"</param>
    ''' <param name="data">Data to be uploaded to the specified resource url.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UploadString(ByVal url As String, ByVal method As String, ByVal data As String) As String
        If String.IsNullOrEmpty(url) Then
            Throw New ArgumentNullException("url", "url cannot be null or empty.")
        End If

        Dim request As HttpWebRequest
        Dim response As HttpWebResponse = Nothing
        Dim responseReader As StreamReader = Nothing

        'Determine the encoding type
        If _encoding Is Nothing Then
            _encoding = Encoding.UTF8
        End If

        Dim dataBytes() As Byte = _encoding.GetBytes(data)

        Try
            request = CType(WebRequest.Create(url), HttpWebRequest)
            request.ContentType = "application/x-www-form-urlencoded"
            request.Timeout = _timeout
            request.ReadWriteTimeout = _timeout
            If _requestHeaders IsNot Nothing Then
                request.Headers = _requestHeaders
            End If
            request.KeepAlive = _keepAlive
            'The default method is POST
            If String.IsNullOrEmpty(method) Then
                request.Method = "POST"
            Else
                request.Method = method
            End If
            request.ProtocolVersion = _httpProtocolVersion

            'Begin uploading
            Using requestStream As Stream = request.GetRequestStream()
                requestStream.Write(dataBytes, 0, dataBytes.Length)
            End Using

            response = CType(request.GetResponse(), HttpWebResponse)
            responseReader = New StreamReader(response.GetResponseStream())
            Return responseReader.ReadToEnd()
        Finally
            If response IsNot Nothing Then
                response.Close()
            End If

            If responseReader IsNot Nothing Then
                responseReader.Close()
            End If
        End Try
    End Function

    ''' <summary>
    ''' Sends a request to a specific Url using POST.
    ''' </summary>
    ''' <param name="url">Location of posting message</param>
    ''' <param name="postData">Sets data fields to be posted to the specified Url</param>
    ''' <returns>Returns response from the server as a string value</returns>
    ''' <remarks>SendData sends data via HTTP protocol using POST method. If you would like to use GET method consider using SendDataUsingGet.</remarks>
    Public Function SendData(ByVal url As String, ByVal postData As System.Collections.Specialized.NameValueCollection) As String
        Return SendData(url, postData, Nothing)
    End Function

    ''' <summary>
    ''' Sends a request to a specific Url.
    ''' </summary>
    ''' <param name="url">Location of posting message</param>
    ''' <param name="postData">Sets data fields to be posted to the specified Url</param>
    ''' <param name="method">Specifies the http request method.</param>
    ''' <returns>Returns response from the server as a string value</returns>
    ''' <remarks></remarks>
    Public Function SendData(ByVal url As String, ByVal method As HttpMethod, ByVal postData As System.Collections.Specialized.NameValueCollection) As String
        If method = HttpMethod.POST Then
            Return sendDataUsingPost(url, postData, Nothing)
        Else
            Return sendDataUsingGet(url, postData)
        End If
    End Function

    ''' <summary>
    ''' Sends a request to a specific url using POST method.
    ''' </summary>
    ''' <param name="url">Location of posting message</param>
    ''' <param name="postData">Sets data fields to be posted to the specified Url</param>
    ''' <param name="postFiles">Sets files to be posted to the specified Url</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendData(ByVal url As String, ByVal postData As System.Collections.Specialized.NameValueCollection, ByVal postFiles As System.Collections.Specialized.NameValueCollection) As String
        Return sendDataUsingPost(url, postData, postFiles)
    End Function

    ''' <summary>
    ''' Sends a request to a specified Url
    ''' </summary>
    ''' <param name="url">Location of posting message</param>
    ''' <param name="postData">Sets data fields to be posted to the specified Url</param>
    ''' <param name="postFiles">Sets files to be posted to the specified Url</param>
    ''' <returns>Returns response from the server as a string value.</returns>
    ''' <remarks>SendData sends data via HTTP protocol using POST method. If you would like to use GET method consider using SendDataUsingGet.</remarks>
    Private Function sendDataUsingPost(ByVal url As String, ByVal postData As System.Collections.Specialized.NameValueCollection, ByVal postFiles As System.Collections.Specialized.NameValueCollection) As String

        If String.IsNullOrEmpty(url) Then
            Throw New ArgumentNullException("url", "url cannot be null or empty.")
        End If

        Dim request As HttpWebRequest = Nothing
        Dim response As HttpWebResponse = Nothing
        Dim responseReader As StreamReader = Nothing
        Dim picStream As FileStream = Nothing

        Dim boundaryHeader As String = "f93dcbA3"
        Dim boundary As String = "--" & boundaryHeader
        Dim footer As String = vbCrLf + boundary + "--" + vbCrLf

        'Build Header
        Dim headerBytes() As Byte
        Dim picFile As FileInfo
        Dim footerBytes() As Byte = _encoding.GetBytes(footer)

        Try
            request = CType(WebRequest.Create(url), HttpWebRequest)
            'Set method
            request.Method = "POST"

            request.ContentType = String.Format("multipart/form-data; boundary={0}", boundaryHeader)
            If _requestHeaders IsNot Nothing Then
                request.Headers = _requestHeaders
            End If
            request.ContentLength = calculateTotalSize(boundary, footer, postData, postFiles)
            request.ReadWriteTimeout = _timeout
            request.KeepAlive = _keepAlive
            request.ProtocolVersion = _httpProtocolVersion
            request.Timeout = _timeout
            Dim requestStream As Stream = request.GetRequestStream()

            If postFiles IsNot Nothing Then
                For Each key As String In postFiles.AllKeys
                    'Write header
                    headerBytes = _encoding.GetBytes(buildFileHeader(boundary, key, postFiles.Get(key)))
                    requestStream.Write(headerBytes, 0, headerBytes.Length)

                    picFile = New FileInfo(postFiles(key))

                    'Write file content
                    picStream = picFile.Open(FileMode.Open, FileAccess.Read)
                    Dim bufferSize As Integer = 2047
                    Dim buffer(bufferSize) As Byte
                    Dim bytesRead As Integer = 0

                    Do
                        bytesRead = picStream.Read(buffer, 0, bufferSize)
                        requestStream.Write(buffer, 0, bytesRead)
                    Loop While bytesRead > 0

                    picStream.Close()
                Next
            End If

            If postData IsNot Nothing Then
                For Each key As String In postData.AllKeys
                    'Write header
                    headerBytes = _encoding.GetBytes(buildDataHeader(boundary, key, postData.Get(key)))
                    requestStream.Write(headerBytes, 0, headerBytes.Length)
                Next
            End If

            'Write footer
            requestStream.Write(footerBytes, 0, footerBytes.Length)
            requestStream.Close()

            response = CType(request.GetResponse(), HttpWebResponse)
            responseReader = New StreamReader(response.GetResponseStream())
            Return responseReader.ReadToEnd()
        Finally
            If response IsNot Nothing Then
                response.Close()
            End If

            If responseReader IsNot Nothing Then
                responseReader.Close()
            End If

            If picStream IsNot Nothing Then
                picStream.Close()
            End If
        End Try
    End Function

    ''' <summary>
    ''' Sends a request to a specified Url
    ''' </summary>
    ''' <param name="url">Location of posting message</param>
    ''' <param name="postData">Sets data fields to be posted to the specified Url</param>
    ''' <returns>Returns response from the server as a string value.</returns>
    ''' <remarks>SendData sends data via HTTP protocol using GET method. If you would like to use POST method consider using SendData.</remarks>
    Private Function sendDataUsingGet(ByVal url As String, ByVal postData As System.Collections.Specialized.NameValueCollection) As String
        If String.IsNullOrEmpty(url) Then
            Throw New ArgumentNullException("url", "url cannot be null or empty.")
        End If

        Dim request As HttpWebRequest = Nothing
        Dim response As HttpWebResponse = Nothing
        Dim responseReader As StreamReader = Nothing

        Try
            'Build the new url
            Dim newUrl As String = String.Empty
            If Not url.Contains("?") Then
                newUrl = url & "?"
            End If

            For Each key As String In postData
                newUrl += String.Format("{0}={1}&", key, postData(key))
            Next

            'Remove the last &
            newUrl = newUrl.Remove(newUrl.Length - 1, 1)

            request = CType(WebRequest.Create(newUrl), HttpWebRequest)
            'Set method
            request.Method = "GET"
            'request.ContentType = "application/x-www-form-urlencoded"
            request.ContentType = "application/xml"
            If _requestHeaders IsNot Nothing Then
                request.Headers = _requestHeaders
            End If
            request.KeepAlive = _keepAlive
            request.ProtocolVersion = _httpProtocolVersion
            request.Timeout = _timeout
            response = CType(request.GetResponse(), HttpWebResponse)
            responseReader = New StreamReader(response.GetResponseStream())
            Return responseReader.ReadToEnd()
        Finally
            'Clean up resources
            If response IsNot Nothing Then
                response.Close()
            End If

            If responseReader IsNot Nothing Then
                responseReader.Close()
            End If
        End Try
    End Function

#End Region

#Region "Logic"

    Public Sub EnableHttpsRequest()
        _keepAlive = False
        _httpProtocolVersion = HttpVersion.Version10
    End Sub

    ''' <summary>
    ''' Verifies data and calculates total size of the data which you can assign the returned value into HttpRequest.ContentLength
    ''' </summary>
    ''' <param name="boundary"></param>
    ''' <param name="postData"></param>
    ''' <param name="postFiles"></param>
    ''' <returns>Returns the total length of the data to be uploaded to the specified url.</returns>
    ''' <remarks></remarks>
    Private Function calculateTotalSize(ByVal boundary As String, ByVal footer As String, ByVal postData As System.Collections.Specialized.NameValueCollection, ByVal postFiles As System.Collections.Specialized.NameValueCollection) As Long
        Dim totalSize As Long
        Dim localFile As FileInfo
        Dim header As String
        Dim footerBytes() As Byte

        'Calculate length of files
        If postFiles IsNot Nothing Then
            For Each key As String In postFiles.AllKeys
                'Calculate file header
                header = buildFileHeader(boundary, key, postFiles(key))
                totalSize += _encoding.GetBytes(header).Length
                'Calculate physical file length
                localFile = New FileInfo(postFiles(key))
                totalSize += localFile.Length
            Next
        End If

        'Calculate length of data
        If postData IsNot Nothing Then
            For Each key As String In postData.AllKeys
                'Calculate data header
                header = buildDataHeader(boundary, key, postData(key))
                totalSize += _encoding.GetBytes(header).Length
            Next
        End If

        footerBytes = _encoding.GetBytes(footer)
        totalSize += footerBytes.Length

        Return totalSize
    End Function

    ''' <summary>
    ''' Builds http request file header.
    ''' </summary>
    ''' <param name="boundary">The fixed boundary used in the current http request.</param>
    ''' <param name="fieldName">Field Name.</param>
    ''' <param name="filePath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function buildFileHeader(ByVal boundary As String, ByVal fieldName As String, ByVal filePath As String) As String
        Dim header As New StringBuilder()
        Dim fileName As String = Path.GetFileName(filePath)
        header.AppendLine()
        header.AppendLine(boundary)
        header.AppendFormat("Content-Disposition: form-data; name=""{0}"";", fieldName)
        header.AppendFormat(" filename=""{0}""", fileName)
        header.AppendLine()
        header.AppendFormat("Content-Type: {0}", getContentType(fileName))
        header.AppendLine()
        header.AppendLine()
        Return header.ToString()
    End Function

    Private Function buildDataHeader(ByVal boundary As String, ByVal fieldName As String, ByVal value As String) As String
        Dim header As New System.Text.StringBuilder()
        header.AppendLine()
        header.AppendLine(boundary)
        header.AppendFormat("Content-Disposition: form-data; name=""{0}""", fieldName)
        header.AppendLine()
        header.AppendLine()
        header.Append(value)

        Return header.ToString()
    End Function

    Private Function getContentType(ByVal fileName As String) As String
        Dim fileExt As String = Path.GetExtension(fileName).Replace(".", "").ToLower()
        Dim contentType As String = String.Empty
        Select Case fileExt
            Case "html", "htm"
                'HTML text data
                contentType = "text/html"
            Case "txt"
                'Plain text
                contentType = "text/plain"
            Case "css"
                'Cascading Sytlesheets
                contentType = "text/css"
            Case "gif"
                contentType = "image/gif"
            Case "png"
                contentType = "image/png"
            Case "jpeg", "jpg", "jpe"
                contentType = "image/jpeg"
            Case "tiff", "tif"
                contentType = "image/tiff"
            Case "bmp"
                contentType = "image/x-ms-bmp"
            Case "wav"
                contentType = "audio/x-wav"
            Case "mpeg", "mpg", "mpe"
                contentType = "video/mpeg"
            Case "qt", "mov"
                contentType = "video/quicktime"
            Case "avi"
                'Microsoft video
                contentType = "video/x-msvideo"
            Case "rtf"
                'Microsoft Rich Text Format
                contentType = "application/rtf"
            Case "pdf"
                'Adobe Acrobat PDF
                contentType = "application/pdf"
            Case "doc", "docx"
                'Microsoft Word Document
                contentType = "application/msword"
            Case "tar"
                contentType = "application/x-tar"
            Case "zip"
                contentType = "application/zip"
            Case "exe", "bin"
                contentType = "application/octet-stream"
            Case Else
                contentType = "application/octet-stream"
        End Select

        Return contentType
    End Function
#End Region

#Region "Properties"

    Public Property Encoding() As System.Text.Encoding
        Get
            Return _encoding
        End Get
        Set(ByVal value As System.Text.Encoding)
            _encoding = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the request timeout.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Timeout() As Integer
        Get
            Return _timeout
        End Get
        Set(ByVal value As Integer)
            _timeout = value

            'Check if the UploadManager is busy
            'If it is busy throw an InvalidOperationException
            If _isBusy Then
                Throw New InvalidOperationException("Cannot set TimeOut while UploadManager is busy")
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the request Http protocol version
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HttpProtocolVersion() As Version
        Get
            Return _httpProtocolVersion
        End Get
        Set(ByVal value As Version)
            If value IsNot HttpVersion.Version10 OrElse value IsNot HttpVersion.Version11 Then
                Throw New ArgumentException("Invalid Http Protocol Version. Use the Version10 or Version11 fields of the System.Net.HttpVersion class.")
            End If

            'Check if the UploadManager is busy
            'If it is busy throw an InvalidOperationException
            If _isBusy Then
                Throw New InvalidOperationException("Cannot set HttpProtocolVersion while UploadManager is busy")
            End If

            _httpProtocolVersion = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets KeepAlive request header
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property KeepAlive() As Boolean
        Get
            Return _keepAlive
        End Get
        Set(ByVal value As Boolean)

            'Check if the UploadManager is busy
            'If it is busy throw an InvalidOperationException
            If _isBusy Then
                Throw New InvalidOperationException("Cannot set KeepAlive while UploadManager is busy")
            End If

            _keepAlive = value
        End Set
    End Property

    Public Property Headers() As WebHeaderCollection
        Get
            Return _requestHeaders
        End Get
        Set(ByVal value As WebHeaderCollection)
            'Check if the UploadManager is busy
            'If it is busy throw an InvalidOperationException
            If _isBusy Then
                Throw New InvalidOperationException("Cannot set Headers while UploadManager is busy")
            End If

            _requestHeaders = value
        End Set
    End Property

    Public ReadOnly Property IsBusy() As Boolean
        Get
            Return _isBusy
        End Get
    End Property
#End Region

End Class
