package kro.kr.rhya_network.utaiteplayer.utils;

import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;

public class RhyaPlayListImageManager {
    public int getImageID(int type) {
        int image;
        switch (type) {
            default:
                image = R.drawable.app_logo_for_black;
                break;
            case 0:
                image = R.drawable.ic_playlist_custom_1;
                break;
            case 1:
                image = R.drawable.ic_playlist_custom_2;
                break;
            case 2:
                image = R.drawable.ic_playlist_custom_3;
                break;
            case 3:
                image = R.drawable.ic_playlist_custom_4;
                break;
            case 4:
                image = R.drawable.ic_playlist_custom_5;
                break;
            case 5:
                image = R.drawable.ic_playlist_custom_6;
                break;
            case 6:
                image = R.drawable.ic_playlist_custom_7;
                break;
            case 7:
                image = R.drawable.ic_playlist_custom_8;
                break;
            case 8:
                image = R.drawable.ic_playlist_custom_9;
                break;
            case 9:
                image = R.drawable.ic_playlist_custom_10;
                break;
            case 10:
                image = R.drawable.ic_playlist_custom_11;
                break;
        }

        return image;
    }


    public ArrayList<RhyaImageData> getImageData() {
        ArrayList<RhyaImageData> listMain = new ArrayList<>();
        listMain.add(new RhyaImageData(R.drawable.ic_playlist_custom_1, 0, false));
        listMain.add(new RhyaImageData(R.drawable.ic_playlist_custom_2, 1,false));
        listMain.add(new RhyaImageData(R.drawable.ic_playlist_custom_3, 2,false));
        listMain.add(new RhyaImageData(R.drawable.ic_playlist_custom_4, 3,false));
        listMain.add(new RhyaImageData(R.drawable.ic_playlist_custom_5, 4,false));
        listMain.add(new RhyaImageData(R.drawable.ic_playlist_custom_6, 5,false));
        listMain.add(new RhyaImageData(R.drawable.ic_playlist_custom_7, 6,false));
        listMain.add(new RhyaImageData(R.drawable.ic_playlist_custom_8, 7,false));
        listMain.add(new RhyaImageData(R.drawable.ic_playlist_custom_9, 8,false));
        listMain.add(new RhyaImageData(R.drawable.ic_playlist_custom_10, 9,false));
        listMain.add(new RhyaImageData(R.drawable.ic_playlist_custom_11, 10,false));

        return listMain;
    }
}
