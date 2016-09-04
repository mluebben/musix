-- 
-- This script will add the needed columns for row version tracking
--
-- Matthias Luebben <ml81@gmx.de>
-- 15.01.2016
--


-- Extend table album
--

ALTER TABLE album
  ADD dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;
ALTER TABLE album
  ADD dtAdded DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;

CREATE INDEX ix_album_dtRowLastModified ON album(dtRowLastModified);

CREATE TABLE xsync__album_histstat(
  idAlbum INT PRIMARY KEY
);

CREATE TABLE xsync__album_histlog(
  idAlbum INT PRIMARY KEY,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX ix_xsync__album_histlog_dtDeleted ON xsync__album_histlog(dtDeleted);


-- Extend table album_artist
--

ALTER TABLE album_artist
  ADD dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;

CREATE INDEX ix_album_artist_dtRowLastModified ON album_artist(dtRowLastModified);

CREATE TABLE xsync__album_artist_histstat(
  idArtist INT,
  idAlbum INT,
  PRIMARY KEY(idAlbum, idArtist)
);

CREATE TABLE xsync__album_artist_histlog(
  idArtist INT,  
  idAlbum INT,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY(idAlbum, idArtist)
);

CREATE INDEX ix_xsync__album_artist_histlog_dtDeleted ON xsync__album_artist_histlog(dtDeleted);


-- Extend table album_artist
--

ALTER TABLE album_genre
  ADD dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;

CREATE INDEX ix_album_genre_dtRowLastModified ON album_genre(dtRowLastModified);

CREATE TABLE xsync__album_genre_histstat(
  idGenre INT,
  idAlbum INT,
  PRIMARY KEY(idAlbum, idGenre)
);

CREATE TABLE xsync__album_genre_histlog(
  idGenre INT,
  idAlbum INT,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY(idAlbum, idGenre)
);

CREATE INDEX ix_xsync__album_genre_histlog_dtDeleted ON xsync__album_genre_histlog(dtDeleted);


-- Extend table art
--

ALTER TABLE art
  ADD dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;

CREATE INDEX ix_art_dtRowLastModified ON art(dtRowLastModified);

CREATE TABLE xsync__art_histstat(
  art_id INT PRIMARY KEY
);

CREATE TABLE xsync__art_histlog(
  art_id INT PRIMARY KEY,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX ix_xsync__art_histlog_dtDeleted ON xsync__art_histlog(dtDeleted);


-- Extend table artist
--

ALTER TABLE artist
  ADD dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;
ALTER TABLE artist
  ADD dtAdded DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;

CREATE INDEX ix_artist_dtRowLastModified ON artist(dtRowLastModified);

CREATE TABLE xsync__artist_histstat(
  idArtist INT PRIMARY KEY
);

CREATE TABLE xsync__artist_histlog(
  idArtist INT PRIMARY KEY,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX ix_xsync__artist_histlog_dtDeleted ON xsync__artist_histlog(dtDeleted);


-- Extend table genre
--

ALTER TABLE genre
  ADD dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;

CREATE INDEX ix_genre_dtRowLastModified ON genre(dtRowLastModified);

CREATE TABLE xsync__genre_histstat(
  idGenre INT PRIMARY KEY
);

CREATE TABLE xsync__genre_histlog(
  idGenre INT PRIMARY KEY,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX ix_xsync__genre_histlog_dtDeleted ON xsync__genre_histlog(dtDeleted);


-- Extend table path
--

ALTER TABLE path
  ADD dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;

CREATE INDEX ix_path_dtRowLastModified ON path(dtRowLastModified);

CREATE TABLE xsync__path_histstat(
  idPath INT PRIMARY KEY
);

CREATE TABLE xsync__path_histlog(
  idPath INT PRIMARY KEY,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX ix_xsync__path_histlog_dtDeleted ON xsync__path_histlog(dtDeleted);


-- Extend table song
--

ALTER TABLE song
  ADD dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;
ALTER TABLE song
  ADD dtAdded DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;

CREATE INDEX ix_song_dtRowLastModified ON song(dtRowLastModified);

CREATE TABLE xsync__song_histstat(
  idSong INT PRIMARY KEY
);

CREATE TABLE xsync__song_histlog(
  idSong INT PRIMARY KEY,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX ix_xsync__song_histlog_dtDeleted ON xsync__song_histlog(dtDeleted);


-- Extend table song_artist
--

ALTER TABLE song_artist
  ADD dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;

CREATE INDEX ix_song_artist_dtRowLastModified ON song_artist(dtRowLastModified);

CREATE TABLE xsync__song_artist_histstat(
  idArtist INT,
  idSong INT,
  PRIMARY KEY(idSong, idArtist)
);

CREATE TABLE xsync__song_artist_histlog(
  idArtist INT,
  idSong INT,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY(idSong, idArtist)
);

CREATE INDEX ix_xsync__song_artist_histlog_dtDeleted ON xsync__song_artist_histlog(dtDeleted);


-- Extend table song_genre
--

ALTER TABLE song_genre
  ADD dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;

CREATE INDEX ix_song_genre_dtRowLastModified ON song_genre(dtRowLastModified);

CREATE TABLE xsync__song_genre_histstat(
  idGenre INT,
  idSong INT,
  PRIMARY KEY(idSong, idGenre)
);

CREATE TABLE xsync__song_genre_histlog(
  idGenre INT,
  idSong INT,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY(idSong, idGenre)
);

CREATE INDEX ix_xsync__song_genre_histlog_dtDeleted ON xsync__song_genre_histlog(dtDeleted);







-- Create table playlist with version tracking
--

CREATE TABLE playlist(
  idPlaylist VARCHAR(36) PRIMARY KEY,
  strName VARCHAR(255) DEFAULT NULL,
  strDescription VARCHAR(255) DEFAULT NULL,
  strOwner VARCHAR(45) DEFAULT NULL,
  strType VARCHAR(45) DEFAULT NULL,
  strSongs VARCHAR(20000) DEFAULT NULL,
  dtAdded DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE INDEX ix_playlist_dtRowLastModified ON playlist(dtRowLastModified);

CREATE TABLE xsync__playlist_histstat(
  idPlaylist VARCHAR(36) PRIMARY KEY
);

CREATE TABLE xsync__playlist_histlog(
  idPlaylist VARCHAR(36) PRIMARY KEY,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX ix_xsync__playlist_histlog_dtDeleted ON xsync__playlist_histlog(dtDeleted);


-- Create table smartplaylist with version tracking
--

CREATE TABLE smartplaylist(
  idSmartPlaylist VARCHAR(36) PRIMARY KEY,
  strName VARCHAR(255) DEFAULT NULL,
  strDescription VARCHAR(4000) DEFAULT NULL,
  iType INT DEFAULT NULL,
  strSql VARCHAR(4000) DEFAULT NULL,
  strOrderBy VARCHAR(255) DEFAULT NULL,
  dtAdded DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE INDEX ix_smartplaylist_dtRowLastModified ON smartplaylist(dtRowLastModified);

CREATE TABLE xsync__smartplaylist_histstat(
  idSmartPlaylist VARCHAR(36) PRIMARY KEY
);

CREATE TABLE xsync__smartplaylist_histlog(
  idSmartPlaylist VARCHAR(36) PRIMARY KEY,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX ix_xsync__smartplaylist_histlog_dtDeleted ON xsync__smartplaylist_histlog(dtDeleted);


-- Create table history with version tracking
--

CREATE TABLE history(
  idHistory VARCHAR(36) PRIMARY KEY,
  idSong INT NOT NULL,
  dtPlayed DATETIME NOT NULL,
  dtRowLastModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE INDEX ix_history_dtRowLastModified ON history(dtRowLastModified);

CREATE TABLE xsync__history_histstat(
  idHistory VARCHAR(36) PRIMARY KEY
);

CREATE TABLE xsync__history_histlog(
  idHistory VARCHAR(36) PRIMARY KEY,
  dtDeleted DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX ix_xsync__history_histlog_dtDeleted ON xsync__history_histlog(dtDeleted);
