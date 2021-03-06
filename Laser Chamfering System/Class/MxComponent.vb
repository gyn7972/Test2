Imports System.Threading
Imports System.Threading.Tasks

Public Class MxComponent
    Private Const Success As Integer = 0
    Private Const WordSize As Integer = 16
    Private Const LoopDelay As Integer = 10

#If PLATFORM = "x64" Then
    Private com_ReferencesUtlType As ActUtlType64Lib.ActUtlType64Class
#Else
    Private com_ReferencesUtlType As ActUtlTypeLib.ActUtlTypeClass
#End If

    Private lWriteBitAddressMap As New List(Of PlcAddressMapModel)
    Private lReadBitAddressMap As New List(Of PlcAddressMapModel)
    Private lWriteWordAddressMap As New List(Of PlcAddressMapModel)
    Private lReadWordAddressMap As New List(Of PlcAddressMapModel)

    Private lWriteBits As New List(Of PlcBitMemoryModel)
    Private lReadBits As New List(Of PlcBitMemoryModel)
    Private lWriteWords As New List(Of PlcWordMemoryModel)
    Private lReadWords As New List(Of PlcWordMemoryModel)

    Private arWriteBitBuffer() As Boolean
    Private arReadBitBuffer() As Boolean
    Private arWriteWordBuffer() As Short
    Private arReadWordBuffer() As Short

    Private bIsOpened As Boolean
    Private objLocker As New Object

    Private Sub ReadWriteBits()
        If Not bIsOpened Then
            Exit Sub
        End If

        Dim nBufferStartIndex As Integer = 0
        For i As Integer = 0 To lWriteBitAddressMap.Count - 1
            If arWriteBitBuffer Is Nothing Then
                Exit For
            End If

            Dim size As Integer = lWriteBitAddressMap(i).nSize / WordSize
            If (size.Equals(0)) Then
                Exit For
            End If

            Dim readDeviceBlock() As Short
            ReDim readDeviceBlock(size - 1)
            If (Not com_ReferencesUtlType.ReadDeviceBlock2(String.Format("B{0:X3}", lWriteBitAddressMap(i).nStartAddress), size, readDeviceBlock(0))) Then
                'Fail
            End If

            If (i > 0) Then
                nBufferStartIndex += lWriteBitAddressMap(i - 1).nSize
            End If
            For j As Integer = 0 To readDeviceBlock.Length - 1
                For k As Integer = 0 To WordSize - 1
                    arWriteBitBuffer(nBufferStartIndex + j * WordSize + k) = If(((readDeviceBlock(j) >> k) And 1) = 1, True, False)
                Next
            Next
        Next
    End Sub

    Private Sub ReadWriteWords()
        If Not bIsOpened Then
            Exit Sub
        End If

        Dim nBufferStartIndex As Integer = 0
        For i As Integer = 0 To lWriteWordAddressMap.Count - 1
            If arWriteWordBuffer Is Nothing Then
                Exit For
            End If

            Dim size As Integer = lWriteWordAddressMap(i).nSize
            If (size.Equals(0)) Then
                Exit For
            End If

            Dim readDeviceBlock() As Short
            ReDim readDeviceBlock(size - 1)
            If (Not com_ReferencesUtlType.ReadDeviceBlock2(String.Format("W{0:X3}", lWriteWordAddressMap(i).nStartAddress), size, readDeviceBlock(0))) Then
                'Fail
            End If

            If i > 0 Then
                nBufferStartIndex += lWriteWordAddressMap(i - 1).nSize
            End If

            For j As Integer = 0 To readDeviceBlock.Length - 1
                arWriteWordBuffer(nBufferStartIndex + j) = readDeviceBlock(j)
            Next
        Next
    End Sub

    Private Sub UpdateWriteBits()
        If Not bIsOpened Then
            Exit Sub
        End If

        Dim nBufferStartIndex As Integer = 0
        For i As Integer = 0 To lWriteBitAddressMap.Count - 1
            If arWriteBitBuffer Is Nothing Then
                Exit For
            End If

            Dim size As Integer = lWriteBitAddressMap(i).nSize / WordSize
            If (size.Equals(0)) Then
                Exit For
            End If

            'Dim writeDeviceBlock() As Short
            Dim writeDeviceBlock() As Integer
            ReDim writeDeviceBlock(arWriteBitBuffer.Length / WordSize - 1)
            If (i > 0) Then
                nBufferStartIndex += lWriteBitAddressMap(i - 1).nSize
            End If
            For j As Integer = 0 To writeDeviceBlock.Length - 1
                For k As Integer = 0 To WordSize - 1
                    writeDeviceBlock(j) = writeDeviceBlock(j) Or (If(arWriteBitBuffer(nBufferStartIndex + j * WordSize + k) = True, 1, 0) << k)
                Next
            Next

            'If (Not com_ReferencesUtlType.WriteDeviceBlock2(String.Format("B{0:X3}", lWriteBitAddressMap(i).nStartAddress), size, writeDeviceBlock(0)).Equals(Success)) Then
            If (Not com_ReferencesUtlType.WriteDeviceBlock(String.Format("B{0:X3}", lWriteBitAddressMap(i).nStartAddress), size, writeDeviceBlock(0)).Equals(Success)) Then
                'Fail
            End If

        Next

        For i As Integer = 0 To lWriteBits.Count - 1
            lWriteBits(i).bValue = arWriteBitBuffer(lWriteBits(i).nIndex)
        Next
    End Sub

    Private Sub UpdateReadBits()
        If Not bIsOpened Then
            Exit Sub
        End If

        Dim nBufferStartIndex As Integer = 0
        For i As Integer = 0 To lReadBitAddressMap.Count - 1
            If arReadBitBuffer Is Nothing Then
                Exit For
            End If

            Dim size As Integer = lReadBitAddressMap(i).nSize / WordSize
            If (size.Equals(0)) Then
                Exit For
            End If

            Dim readDeviceBlock() As Short
            ReDim readDeviceBlock(size - 1)
            If (Not com_ReferencesUtlType.ReadDeviceBlock2(String.Format("B{0:X3}", lReadBitAddressMap(i).nStartAddress), size, readDeviceBlock(0))) Then
                'Fail
            End If

            If i > 0 Then
                nBufferStartIndex += lReadWordAddressMap(i - 1).nSize
            End If

            For j As Integer = 0 To readDeviceBlock.Length - 1
                For k As Integer = 0 To WordSize - 1
                    arReadBitBuffer(nBufferStartIndex + j * WordSize + k) = If(((readDeviceBlock(j) >> k) And 1) = 1, True, False)
                Next
            Next
        Next

        For i As Integer = 0 To lReadBits.Count - 1
            lReadBits(i).bValue = arReadBitBuffer(i)
        Next

    End Sub

    Private Sub UpdateWriteWords()
        If Not bIsOpened Then
            Exit Sub
        End If

        Dim nBufferStartIndex As Integer = 0
        For i As Integer = 0 To lWriteWordAddressMap.Count - 1
            If arWriteWordBuffer Is Nothing Then
                Exit For
            End If

            Dim size As Integer = lWriteWordAddressMap(i).nSize
            If (size.Equals(0)) Then
                Exit For
            End If

            'Dim writeDeviceBlock() As Short
            Dim writeDeviceBlock() As Integer
            ReDim writeDeviceBlock(size - 1)

            If (i > 0) Then
                nBufferStartIndex += lWriteWordAddressMap(i - 1).nSize
            End If
            For j As Integer = 0 To writeDeviceBlock.Length - 1
                writeDeviceBlock(j) = arWriteWordBuffer(nBufferStartIndex + j)
            Next

            'If (Not com_ReferencesUtlType.WriteDeviceBlock2(String.Format("W{0:X3}", lWriteWordAddressMap(i).nStartAddress), size, writeDeviceBlock(0)).Equals(Success)) Then
            If (Not com_ReferencesUtlType.WriteDeviceBlock(String.Format("W{0:X3}", lWriteWordAddressMap(i).nStartAddress), size, writeDeviceBlock(0)).Equals(Success)) Then
                'Fail
            End If
        Next

        For i As Integer = 0 To lWriteWords.Count - 1
            lWriteWords(i).sValue = arWriteWordBuffer(lWriteWords(i).nIndex)
        Next
    End Sub

    Private Sub UpdateReadWords()
        If Not bIsOpened Then
            Exit Sub
        End If

        Dim nBufferStartIndex As Integer = 0
        For i As Integer = 0 To lReadWordAddressMap.Count - 1
            If arReadWordBuffer Is Nothing Then
                Exit For
            End If

            Dim size As Integer = lReadWordAddressMap(i).nSize
            If (size.Equals(0)) Then
                Exit For
            End If

            Dim readDeviceBlock() As Short
            ReDim readDeviceBlock(size - 1)
            If (Not com_ReferencesUtlType.ReadDeviceBlock2(String.Format("W{0:X3}", lReadWordAddressMap(i).nStartAddress), size, readDeviceBlock(0))) Then
                'Fail
            End If

            If i > 0 Then
                nBufferStartIndex += lReadWordAddressMap(i - 1).nSize
            End If

            For j As Integer = 0 To readDeviceBlock.Length - 1
                arReadWordBuffer(nBufferStartIndex + j) = readDeviceBlock(j)
            Next
        Next

        For i As Integer = 0 To lReadWords.Count() - 1
            lReadWords(i).sValue = arReadWordBuffer(i)
        Next
    End Sub

    Private Function RunAsync() As Boolean
        ReadWriteBits()
        ReadWriteWords()
        Dim sw As Stopwatch = New Stopwatch()
        sw.Start()

        While bIsOpened
            Dim startTime As Long = sw.ElapsedMilliseconds

            SyncLock objLocker
                UpdateWriteBits()
                UpdateReadBits()
                UpdateWriteWords()
                UpdateReadWords()
            End SyncLock

            System.Threading.Thread.Sleep(LoopDelay)

            Dim endTime As Long = sw.ElapsedMilliseconds

            System.Diagnostics.Debug.WriteLine(String.Format("Time : {0}", endTime - startTime))
        End While

        Return True
    End Function

    Public Sub Initializing(ByVal _filePath As String)
#If PLATFORM = "x64" Then
        com_ReferencesUtlType = New ActUtlType64Lib.ActUtlType64Class
#Else
        com_ReferencesUtlType = New ActUtlTypeLib.ActUtlTypeClass
        AddHandler com_ReferencesUtlType.OnDeviceStatus, AddressOf ActEvent_OnDeviceStatus
#End If

        lWriteBitAddressMap.Clear()
        lReadBitAddressMap.Clear()
        lWriteWordAddressMap.Clear()
        lReadWordAddressMap.Clear()

        lWriteBits.Clear()
        lReadBits.Clear()
        lWriteWords.Clear()
        lReadWords.Clear()

        Using reader As New System.IO.StreamReader(_filePath, System.Text.Encoding.UTF8)
            Dim currentRow As String

            While Not reader.EndOfStream
                currentRow = reader.ReadLine()
                currentRow = currentRow.Replace(Convert.ToChar(0), "")

                If (String.IsNullOrEmpty(currentRow)) Then
                    Exit While
                End If

                If (currentRow.Equals("TYPE,START_ADDRESS,SIZE,")) Then
                    While True
                        currentRow = reader.ReadLine()
                        currentRow = currentRow.Replace(Convert.ToChar(0), "")

                        If (String.IsNullOrWhiteSpace(currentRow)) Then
                            Exit While
                        End If

                        Dim splitted() As String = currentRow.Split(","c)
                        If (String.IsNullOrWhiteSpace(splitted(0))) Then
                            Exit While
                        End If
                        Dim addressMap As PlcAddressMapModel
                        addressMap = New PlcAddressMapModel
                        addressMap.nStartAddress = Convert.ToInt32(splitted(1), 16)
                        addressMap.nSize = Convert.ToInt32(splitted(2), 16)
                        If splitted(0).Equals("WRITE_BIT") Then
                            lWriteBitAddressMap.Add(addressMap)
                        ElseIf splitted(0).Equals("READ_BIT") Then
                            lReadBitAddressMap.Add(addressMap)
                        ElseIf splitted(0).Equals("WRITE_WORD") Then
                            lWriteWordAddressMap.Add(addressMap)
                        ElseIf splitted(0).Equals("READ_WORD") Then
                            lReadWordAddressMap.Add(addressMap)
                        End If
                    End While

                ElseIf (currentRow.Equals("TYPE,INDEX,ADDRESS,COMMENT")) Then
                    While True
                        currentRow = reader.ReadLine()
                        currentRow = currentRow.Replace(Convert.ToChar(0), "")

                        If (String.IsNullOrWhiteSpace(currentRow)) Then
                            Exit While
                        End If

                        Dim splitted() As String = currentRow.Split(","c)
                        If (String.IsNullOrWhiteSpace(splitted(0))) Then
                            Exit While
                        End If
                        If splitted(0).Equals("WRITE_BIT") Then
                            Dim memory As PlcBitMemoryModel
                            memory = New PlcBitMemoryModel
                            memory.nIndex = Convert.ToInt32(splitted(1))
                            memory.nAddress = Convert.ToInt32(splitted(2), 16)
                            memory.strComment = splitted(3)
                            memory.bValue = False
                            lWriteBits.Add(memory)
                        ElseIf splitted(0).Equals("READ_BIT") Then
                            Dim memory As PlcBitMemoryModel
                            memory = New PlcBitMemoryModel
                            memory.nIndex = Convert.ToInt32(splitted(1))
                            memory.nAddress = Convert.ToInt32(splitted(2), 16)
                            memory.strComment = splitted(3)
                            memory.bValue = False
                            lReadBits.Add(memory)
                        ElseIf splitted(0).Equals("WRITE_WORD") Then
                            Dim memory As PlcWordMemoryModel
                            memory = New PlcWordMemoryModel
                            memory.nIndex = Convert.ToInt32(splitted(1))
                            memory.nAddress = Convert.ToInt32(splitted(2), 16)
                            memory.strComment = splitted(3)
                            memory.sValue = 0
                            lWriteWords.Add(memory)
                        ElseIf splitted(0).Equals("READ_WORD") Then
                            Dim memory As PlcWordMemoryModel
                            memory = New PlcWordMemoryModel
                            memory.nIndex = Convert.ToInt32(splitted(1))
                            memory.nAddress = Convert.ToInt32(splitted(2), 16)
                            memory.strComment = splitted(3)
                            memory.sValue = 0
                            lReadWords.Add(memory)
                        Else
                            Continue While
                        End If
                    End While
                End If
            End While
        End Using

        Dim totalSize As Integer = 0
        totalSize = 0
        For i As Integer = 0 To lWriteBitAddressMap.Count - 1
            totalSize += lWriteBitAddressMap(i).nSize
        Next
        ReDim arWriteBitBuffer(totalSize - 1)
        totalSize = 0
        For i As Integer = 0 To lReadBitAddressMap.Count - 1
            totalSize += lReadBitAddressMap(i).nSize
        Next
        ReDim arReadBitBuffer(totalSize - 1)
        totalSize = 0
        For i As Integer = 0 To lWriteWordAddressMap.Count - 1
            totalSize += lWriteWordAddressMap(i).nSize
        Next
        ReDim arWriteWordBuffer(totalSize - 1)
        totalSize = 0
        For i As Integer = 0 To lReadWordAddressMap.Count - 1
            totalSize += lReadWordAddressMap(i).nSize
        Next
        ReDim arReadWordBuffer(totalSize - 1)

    End Sub

    Public Sub Finalizing()
#If PLATFORM = "x86" Then
        'Clear Event handler.
        RemoveHandler com_ReferencesUtlType.OnDeviceStatus, AddressOf ActEvent_OnDeviceStatus
#End If
        com_ReferencesUtlType = Nothing
    End Sub

    Public Function Open(ByVal _nStationNumber As Integer) As Boolean
        If (bIsOpened) Then
            Return True
        End If

        com_ReferencesUtlType.ActLogicalStationNumber = _nStationNumber

        Dim iResult As Integer
        iResult = com_ReferencesUtlType.Open()
        If (Not iResult.Equals(Success)) Then
            Return False
        End If

        bIsOpened = True

        Task.Run(Function() RunAsync())

        Return True
    End Function

    Public Function Close()
        If (Not bIsOpened) Then
            Return True
        End If

        bIsOpened = False

        If (Not com_ReferencesUtlType.Close().Equals(Success)) Then
            Return False
        End If

        Return True
    End Function

    Public Function IsOpened() As Boolean
        Return bIsOpened
    End Function

    Public Function GetWriteBits()
        Return lWriteBits
    End Function

    Public Function GetReadBits()
        Return lReadBits
    End Function

    Public Function GetWriteWords()
        Return lWriteWords
    End Function

    Public Function GetReadWords()
        Return lReadWords
    End Function

    Public Function WriteBit(ByVal _strComment As String, ByVal _value As Boolean)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundBit = lWriteBits.Find(Function(x) x.strComment.Equals(_strComment))
        If foundBit Is Nothing Then
            Return False
        End If

        SyncLock objLocker
            arWriteBitBuffer(foundBit.nIndex) = _value
        End SyncLock

        Return True
    End Function

    'Public Function ReadBit(ByVal _strComment As String, ByRef _value As Boolean)
    '    If Not bIsOpened Then
    '        Return False
    '    End If

    '    Dim foundBit = lReadBits.Find(Function(x) x.strComment.Equals(_strComment))
    '    If foundBit Is Nothing Then
    '        Return False
    '    End If

    '    _value = arReadBitBuffer(foundBit.nIndex)
    '    Return True
    'End Function

    Public Function ReadBit(ByVal _strComment As String, ByRef _value As Boolean)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundBit = lReadBits.Find(Function(x) x.strComment.Equals(_strComment))
        If foundBit Is Nothing Then
            Return False
        End If

        _value = arReadBitBuffer(foundBit.nIndex)
        Return True
    End Function

    Public Function WriteWord(ByVal _strComment As String, ByVal _value As Short)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundWord = lWriteWords.Find(Function(x) x.strComment.Equals(_strComment))
        If foundWord Is Nothing Then
            Return False
        End If

        SyncLock objLocker
            arWriteWordBuffer(foundWord.nIndex) = _value
        End SyncLock

        Return True
    End Function

    Public Function ReadWord(ByVal _strComment As String, ByRef _value As Short)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundWord = lReadWords.Find(Function(x) x.strComment.Equals(_strComment))
        If foundWord Is Nothing Then
            Return False
        End If

        _value = arReadWordBuffer(foundWord.nIndex)
        Return True
    End Function

    Public Function WriteBitByAddress(ByVal _address As Integer, ByVal _value As Boolean)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundBit = lWriteBits.Find(Function(x) x.nAddress.Equals(_address))
        If foundBit Is Nothing Then
            Return False
        End If

        SyncLock objLocker
            arWriteBitBuffer(foundBit.nIndex) = _value
        End SyncLock

        Return True
    End Function

    Public Function WriteBitReadAddress(ByVal _address As Integer, ByVal _value As Boolean)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundBit = lWriteBits.Find(Function(x) x.nAddress.Equals(_address))
        If foundBit Is Nothing Then
            Return False
        End If

     
        _value = arWriteBitBuffer(foundBit.nIndex)

        Return True
    End Function

    Public Function ReadBitByAddress(ByVal _address As Integer, ByRef _value As Boolean)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundBit = lReadBits.Find(Function(x) x.nAddress.Equals(_address))
        If foundBit Is Nothing Then
            Return False
        End If

        _value = arReadBitBuffer(foundBit.nIndex)
        Return _value
    End Function

    Public Function WriteWordShortByAddress(ByVal _address As Integer, ByVal _value As Short)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundWord = lWriteWords.Find(Function(x) x.nAddress.Equals(_address))
        If foundWord Is Nothing Then
            Return False
        End If

        SyncLock objLocker
            arWriteWordBuffer(foundWord.nIndex) = _value
        End SyncLock

        Return True
    End Function

    Public Function WriteWordIntegerByAddress(ByVal _address As Integer, ByVal _value As Integer)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundWord = lWriteWords.Find(Function(x) x.nAddress.Equals(_address))
        If foundWord Is Nothing Then
            Return False
        End If

        Dim intValue = _value

        SyncLock objLocker
            'arWriteWordBuffer(foundWord.nIndex) = (intValue And &HFFFF0000) >> 16
            'arWriteWordBuffer(foundWord.nIndex + 1) = intValue And &HFFFF
            arWriteWordBuffer(foundWord.nIndex) = intValue
            'arWriteWordBuffer(foundWord.nIndex +  intValue
        End SyncLock

        Return True
    End Function

    Public Function WriteWordDoubleByAddress(ByVal _address As Integer, ByVal _value As Double)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundWord = lWriteWords.Find(Function(x) x.nAddress.Equals(_address))
        If foundWord Is Nothing Then
            Return False
        End If

        Dim doubleValue = (Math.Round(_value, 3) * 1000)
        Dim intValue = Convert.ToInt32(doubleValue)

        SyncLock objLocker
            arWriteWordBuffer(foundWord.nIndex) = (intValue And &HFFFF0000) >> 16
            arWriteWordBuffer(foundWord.nIndex + 1) = intValue And &HFFFF
        End SyncLock

        Return True
    End Function

    Public Function ReadWordShortByAddress(ByVal _address As Integer, ByRef _value As Short)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundWord = lReadWords.Find(Function(x) x.nAddress.Equals(_address))
        If foundWord Is Nothing Then
            Return False
        End If

        _value = arReadWordBuffer(foundWord.nIndex)
        Return True
    End Function

    Public Function ReadWordIntegerByAddress(ByVal _address As Integer, ByRef _value As Integer)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundWord = lReadWords.Find(Function(x) x.nAddress.Equals(_address))
        If foundWord Is Nothing Then
            Return False
        End If

        _value = (arReadWordBuffer(foundWord.nIndex) << 16) + arReadWordBuffer(foundWord.nIndex + 1)
        Return True
    End Function

    Public Function ReadWordDoubleByAddress(ByVal _address As Integer, ByRef _value As Double)
        If Not bIsOpened Then
            Return False
        End If

        Dim foundWord = lReadWords.Find(Function(x) x.nAddress.Equals(_address))
        If foundWord Is Nothing Then
            Return False
        End If

        _value = pLDLT.ConvertWordDataToStageData(arReadWordBuffer(foundWord.nIndex), arReadWordBuffer(foundWord.nIndex + 1)) / 1000.0

        '_value = ((arReadWordBuffer(foundWord.nIndex) << 16) + arReadWordBuffer(foundWord.nIndex + 1)) / 1000.0
        Return True
    End Function
End Class

Public Class PlcAddressMapModel
    Public nStartAddress As Integer
    Public nSize As Integer
End Class

Public Class PlcBitMemoryModel
    Public nIndex As Integer
    Public nAddress As Integer
    Public strComment As String
    Public bValue As Boolean
End Class

Public Class PlcWordMemoryModel
    Public nIndex As Integer
    Public nAddress As Integer
    Public strComment As String
    Public sValue As Short
End Class