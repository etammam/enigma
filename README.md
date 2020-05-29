# string-encryptor
secure string with encryption
##Usage
install this pakage using nuget package manager   
`PM> install-package wikiux.string.encryptor`   
this is static string methods, you can call it directly as .ToEncrypt(); or .ToDecrypt();
```cs
public void TestUsage()
{
    string text = "this is testing string";
    string encryptedText = text.ToEncrypt();
    string decryptedText = encryptedText.ToDecrypt();
}
```
&copy;Wikiux inc. by Eslam M. Tammam
