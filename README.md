# Enigma
secure string with encryption based on symmetric key
##Usage
install this pakage using nuget package manager   
`PM> install-package Fondness.Enigma`   
configure you application to use the engima encryption engin by configure the core service
```cs
_services.AddEnigmaEncryption(options =>
{
    options.SymmetricKey = "b14ca5898a4e4133bbce2ea2315a1916";
});
```
then you can use encryption and decryption extension as a string extension
```cs
string value = "Hello World!";
var encryptedValue = value.Encrypt();

var decryptValue = encryptedValue.Decrypt();
```
&copy;Fondness Open Source.
