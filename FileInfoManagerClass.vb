'*****************************************************************************
'*                    FILE INFO MANAGER FOR PI SYSTEM                        *
'*               Copyright 2018 - Fabiano Batista - OSIsoft                  *
'*                                                                           *
'* Licensed under the Apache License, Version 2.0 (the "License");           *
'* you may not use this file except in compliance with the License.          *
'* You may obtain a copy of the License at                                   *
'*                                                                           *
'*                http://www.apache.org/licenses/LICENSE-2.0                 *
'*                                                                           *
'* Unless required by applicable law or agreed to in writing, software       *
'* distributed under the License is distributed on an "AS IS" BASIS,         *
'* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  *
'* See the License for the specific language governing permissions and       *
'* limitations under the License.                                            *
'*                                                                           *
'* Program Description:                                                      *
'*  This application will monitor the trigger tag. When the a new event is   *
'*  received by the trigger tag, the most recent file which creation date is *
'*  before the trigger event timestamp will be copied to the destination     *
'*  folder, and the full path of the copied file will be written to the      *
'*  output tag.                                                              *                                                                         *
'*****************************************************************************

Imports OSIsoft.PI.ACE
Imports PISDK
Imports PITimeServer
Imports System.IO


Public Class FileInfoManagerClass
    Inherits PIACENetClassModule
    Private Processed_File_Count As PIACEPoint
    Private File_Path As PIACEPoint
    '
    '      Tag Name/VB Variable Name Correspondence Table
    ' Tag Name                                VB Variable Name
    ' ------------------------------------------------------------
    '


    'Global Variables to store application settings
    Dim _sourceFolderPath As String
    Dim _destinationFolderPath As String
    Dim _fileFilterCriteria As String

    'Flag to indicate if files are still being processed
    Dim _isBusy As Boolean = False


    'Reference to current calculation context
    Dim _currentModule As PISDK.PIModule

	' File Path                               File_Path
	' Processed File Count                    Processed_File_Count

    Public Overrides Sub ACECalculations()


        
        'Only files with the defined extension will be processed

        'Holds the text to be sent to the output tag
        Dim outputTagString As String = String.Empty
        Dim processedFileCount As Integer = 0

        'Resetting "SendDataToPI(" attributes")
        Processed_File_Count.SendDataToPI = True
        File_Path.SendDataToPI = True

        Try

         

            'The folder will be processed by the current execution only if there the previous execution has already been completed
            If Not _isBusy Then

                'Scanning source folder
                Dim directory As New System.IO.DirectoryInfo(_sourceFolderPath)
                Dim files As System.IO.FileInfo() = directory.GetFiles(_fileFilterCriteria)

                'Processing each file in the source folder
                For Each file In files
                    _isBusy = True

                    'Getting creating a timestamp set with the file creation time
                    Dim outputTimestamp = New PITimeServer.PITime()
                    outputTimestamp.LocalDate = file.CreationTime


                    Dim fileName As String = file.Name

                    outputTagString = _destinationFolderPath & fileName


                    'Sets the output value and timestamp
                    File_Path.Value(file.CreationTime) = outputTagString

                    'Sends result PI Data Archive right away
                    File_Path.PutValue()

                    processedFileCount = processedFileCount + 1


                    Try

                        'If file with same name exists in the destination, delete if first
                        If System.IO.File.Exists(outputTagString) Then
                            System.IO.File.Delete(outputTagString)
                        End If

                        'Moving file from source folder to destination folder
                        file.MoveTo(outputTagString)


                    Catch ex As Exception
                        PIACEBIFunctions.LogPIACEMessage(OSIsoft.PI.ACE.MessageLevel.mlErrors, ex.Message, MyBase.Context)

                    End Try

                Next


                Processed_File_Count.Value = processedFileCount

                'Since no file was processed, do not send value to "File Path" tag
                File_Path.SendDataToPI = False

                'Indicating that the current execution has already been completed
                _isBusy = False

            Else
               

                'Do not update the output tags since ACE was busy processing previous calculation

                File_Path.SendDataToPI = False
                PIACEBIFunctions.LogPIACEMessage(OSIsoft.PI.ACE.MessageLevel.mlDebug, _
                                "Input folder was not processed_because previous calculation was still active.", MyBase.Context)

            End If


        Catch ex As Exception

            PIACEBIFunctions.LogPIACEMessage(OSIsoft.PI.ACE.MessageLevel.mlErrors, ex.Message, MyBase.Context)
        End Try




    End Sub


    Protected Overrides Sub InitializePIACEPoints()
		File_Path = GetPIACEPoint("File_Path")
		Processed_File_Count = GetPIACEPoint("Processed_File_Count")
    End Sub

    '
    ' User-written module dependent initialization code
    '
    Protected Overrides Sub ModuleDependentInitialization()

        'Getting the reference to the current module
        _currentModule = PIACEBIFunctions.GetPIModuleFromPath(MyBase.Context)

        Try
            'Reading source / destination folder locations and other program parameters
            _sourceFolderPath = _currentModule.PIProperties.Item("Source Folder").Value.ToString
            _destinationFolderPath = _currentModule.PIProperties.Item("Destination Folder").Value.ToString
            _fileFilterCriteria = _currentModule.PIProperties.Item("File Name Filter").Value.ToString

            'Making sure the path has the "\" suffix
            If Not _sourceFolderPath.EndsWith("\") Then

                _sourceFolderPath = _sourceFolderPath & "\"
                Console.WriteLine("Test...")
            End If

            If Not _destinationFolderPath.EndsWith("\") Then

                _destinationFolderPath = _destinationFolderPath & "\"

            End If


        Catch ex As Exception

            PIACEBIFunctions.LogPIACEMessage(OSIsoft.PI.ACE.MessageLevel.mlErrors, ex.Message, MyBase.Context)


        End Try
    End Sub

    '
    ' User-written module dependent termination code
    '
    Protected Overrides Sub ModuleDependentTermination()
    End Sub
End Class
