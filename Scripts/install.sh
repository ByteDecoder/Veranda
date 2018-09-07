#! /bin/sh

BASE_URL=https://download.unity3d.com/download_unity
HASH=f2cce2a5991f
VERSION=2017.4.10f1

install() {
	PACKAGE=$1
	download "$PACKAGE"
	
	echo "Installing "`basename "$PACKAGE"`
	
}

download() {
    FILE=$1
    URL="$BASE_URL/$HASH/$FILE"

    #download package if it does not already exist in cache
    if [ ! -e $UNITY_DOWNLOAD_CACHE/`basename "$FILE"` ] ; then
        echo "$FILE does not exist. Downloading from $URL: "
        curl -o $UNITY_DOWNLOAD_CACHE/`basename "$FILE"` "$URL"
    else
        echo "$FILE Exists. Skipping download."
    fi
}

install() {
  PACKAGE=$1
  download "$PACKAGE"

  echo "Installing "`basename "$package"`
  sudo installer -dumplog -package $UNITY_DOWNLOAD_CACHE/`basename "$PACKAGE"` -target /
}

install "MacEditorInstaller/Unity-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Windows-Support-for-Editor-$VERSION.pkg"