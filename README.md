# IF3210-2021-Unity-K3-14

# Tank Extended Multiplayer

## Deskripsi Aplikasi

Aplikasi yang dibuat adalah game multiplayer dengan fitur-fitur tambahan dari tugas besar IF3210.

## Cara Kerja

1. Pada awalnya, akan diminta untuk memasukkan nama pemain terlebih dahulu. Setelah memasukkan nama, perlu ada player yang menjadi server + client dan ada player yang lain menjadi client. 
![Input Name] (./Images/InputName.png)

2. Setelah itu, akan muncul screen utama untuk memulai game.
![Screen Utama] (./Images/ScreenUtama.png)

3. Jika ingin mengatur suara, bisa menekan tombol suara di samping Start Game. Maka, akan muncul Canvas untuk mengatur suara SFX, musik atau driving. Tekan "X" untuk keluar modifikasi suara.
![Music Canvas] (./Images/MusicCanvas.png)

4. Jika sudah ingin main, langsung tekan "Start Game" maka akan muncul dua pilihan map yaitu "Blizzard" sama "Desert". Silahkan pilih salah satu.
![Choose Map] (./Images/ChooseMap.png)
![Blizzard Map] (./Images/BlizzardMap.png)
![Desert Map] (./Images/DesertMap.png)

5. Di dalam Map, akan muncul gold-gold yang bertebaran secara periodik. Player bisa bergerah ke gold tersebut untuk menambahkan uangnya. Jika uangnya sudah cukup, player bisa mengganti pelurunya dengan menekan tombol tertentu yang yang ada di layar Help.
![Choose Weapon] (./Images/ChooseWeapon.png)

6. Selain membeli atau mengganti pelurunya, player dapat membiayai dua jenis karakter yaitu Infantry dan Bomber yang sudah mempunyai karakteristik yang berbeda. Infantry dapat menambak lawan sedangkan Bomber dapat mengebom diri di dekat lawan. Cara membeli dua karakter ini adalah dengan menekan tombol tertentu yang ada di layar Help. Di dalam game juga ada ditampilkannya berapa jumlah karakter tertentu yang dimiliki player.
![Character 1] (./Images/Character1.png)
![Character 2] (./Images/Character2.png)
![Character 3] (./Images/Character3.png)
![Character 4] (./Images/Character4.png)
![Character 5] (./Images/Character5.png)
![Character 6] (./Images/Character6.png)

7. Pada game ini, tank bisa mendorong karakter dan tank lawan.

8. Mainlah sampai 5 ronde dan player yang memenangkan ronde terbanyak adalah pemenangnya.

## Library and Asset

1. Mirror
Mirror digunakan sebagai library untuk multiplayer
2. Cash Prefab: https://assetstore.unity.com/packages/3d/props/gold-coins-1810
Prefab ini digunakan sebagai model gold
3. Character Animation: https://assetstore.unity.com/packages/3d/animations/melee-axe-pack-35320
Animation ini digunakan sebagai animasi untuk kedua karakter pada game
4. Character Prefab: https://assetstore.unity.com/packages/3d/characters/robots/robot-soldier-142438
Prefab ini digunakan sebagai model untuk karakter

## Pembagian Kerja Kelompok

1. Daniel Riyanto-13518075
- LAN Multiplayer
- Testing
- Readme

2. Vincent Hasiholan-13518108

3. Yonatan Viody - 13518120
- LAN Multiplayer
- Intensitas suara & Nama Pemain
- Dua desain map
- Objek cash
- Dua jenis senjata
- Dua jenis karakter
- Animasi
- Interaksi collision