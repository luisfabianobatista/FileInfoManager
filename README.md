# FileInfoManager
PI ACE application to store file metadata in PI. The file format is generic (it can be image, sound, etc.) 

How to store image files in PI?

Instead of storing the actual image files, the recommended approach is
to simply store the reference to the file.

PI ACE calculation used to capture file metadata (creation date, name
and location) and store it in a PI Tag.

The calculation settings is stored in PI Module Database. You will need
the following Properties:
"Destination Folder" (string to set where the files will be moved to)
"Source Folder" (string containing the name where the pictures will be
saved by the camera)
"Fine Name Filter" (only the files satisfying this name filter condition
will be considered - ex: *.jpg)

Aliases:
"File Path" (output tag - create it as string - it will contain the
name/path of the processed files)
"Processed File Count" (output tag - create it as Int32 - it will
contain the number of files processed each time the calculation is
executed)

Licensing

Copyright 2018 Luis Fabiano Batista, OSIsoft, LLC.

Licensed under the Apache License, Version 2.0 (the "License"); you may
not use this file except in compliance with the License. You may obtain
a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Please see the file named LICENSE.
