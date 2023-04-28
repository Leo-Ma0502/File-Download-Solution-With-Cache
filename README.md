What is this:
a file download app involving cache server.

How does this solution work:
On server side, admin chooses files to be available for download from local files, which in this context are images. When receiving request forwarded from cache server, it breaks the target file into blocks using Rabin function, and sends them to the cache server.

On cache server, it forwards request from client to origin server and computes hash value of each block to check whether it has been cached. If the block has been cached, the cache server sends the hash value to client and if not, both hash value and the block content would be sent. 

On client side, it requests for download for a specific file and assembles blocks either sent by the cache server or indexed by hash value from its local cache.

How to run this program:
git clone, and rebuild the solution. Debug the solution in IDE or double click the start.bat.

• Steps to run:
1. Double click start.bat in the folder, without changing the folder structure.
You would see two windows (Client and Cache), a directory selection dialog, and one
cmd prompt pop up.
2. Choose the folder where you store the test files in the directory selection dialog and
click OK, you would see test file names in the File Server Window;
3. Select files and click make available button to make the files available for download, you may use “control + mouse” to make multiple selections, and your every choice
would overwrite the last one.
4. In Client window, click “Load or reload available files”, you would see files available in
the list view below, remember to click this button whenever making changes to test
files.
5. Click to select one available file (you may only choose one file each time) and click
download button below to download the file.
6. The Client window may show “not responding”, and that is normal because this
process is waiting for the cache to write cached contents, which you may find in the
Cache window.
7. After downloading, you may click to select one of items in the cached list and click
“check selected block content” to view the byte array cached. You may also find the
logs in this window. Click “Clear Cache” button to clear cached contents if you’d like to.
8. Now you may download another file available or make changes to the test files and
repeat the steps above.
