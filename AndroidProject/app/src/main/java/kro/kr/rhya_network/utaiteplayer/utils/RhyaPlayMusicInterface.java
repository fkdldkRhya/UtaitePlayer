package kro.kr.rhya_network.utaiteplayer.utils;

public interface RhyaPlayMusicInterface {
    void onTrackPrevious();
    void onTrackPlay();
    void onTrackPause();
    void onTrackNext(RhyaMusicInfoVO rhyaMusicInfoVOInput, boolean createDialog);
}
