package kro.kr.rhya_network.utaiteplayer.fragment;

import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.appcompat.widget.LinearLayoutCompat;
import androidx.fragment.app.Fragment;
import androidx.viewpager2.widget.ViewPager2;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.Animation;
import android.view.animation.AnimationUtils;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import com.pnikosis.materialishprogress.ProgressWheel;
import com.tbuonomo.viewpagerdotsindicator.WormDotsIndicator;

import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaSearchTopMusicAdapter;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.lib.com_github_amarjain07_StickyScrollView_1_0_2.ui.interfaces.IScrollViewListener;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaDialogManager;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSingerDataVO;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link SearchFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class SearchFragment extends Fragment {
    public SearchFragment() {
        // Required empty public constructor
    }

    // UI Object
    private View viewLayout;
    private TextView subscribeIndex = null;
    private TextView subscribeName = null;
    private LinearLayoutCompat subscribeLayout = null;
    public ViewPager2 viewPager2 = null;
    public ProgressWheel progressWheel = null;
    public ImageView loadImageView = null;
    // Fragment
    private SearchTopSongFragment searchTopSongFragment1 = null;
    private SearchTopSongFragment searchTopSongFragment2 = null;
    // Utils
    private RhyaDialogManager rhyaDialogManager;



    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @return A new instance of fragment SearchFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static SearchFragment newInstance() {
        return new SearchFragment();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_search, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        rhyaDialogManager = new RhyaDialogManager(requireActivity());

        subscribeIndex = view.findViewById(R.id.subscribeIndex);
        subscribeName = view.findViewById(R.id.subscribeName);
        subscribeLayout = view.findViewById(R.id.subscribeLayout);
        progressWheel = view.findViewById(R.id.progressWheel);
        loadImageView = view.findViewById(R.id.loadImageView);
        LinearLayoutCompat subscribeLayout = view.findViewById(R.id.subscribeLayout);
        TextView searchTextView = view.findViewById(R.id.searchTextView);
        ImageView searchImageView = view.findViewById(R.id.searchImageView);
        View _view = view.findViewById(R.id.view);
        kro.kr.rhya_network.utaiteplayer.lib.com_github_amarjain07_StickyScrollView_1_0_2.ui.StickyScrollView stickyScrollView = view.findViewById(R.id.nestedScrollView);
        viewLayout = view.findViewById(R.id.viewSelector);


        viewPager2 = view.findViewById(R.id.mainViewPager);


        // 구독자 리스트 표시
        final String noSubscribeText = "구독 중인 사람 없음";
        ArrayList<RhyaSingerDataVO> subscribeList = RhyaApplication.rhyaMusicDataVO.getSubscribeList();
        if (subscribeList != null) {
            if (subscribeList.size() != 0) {
                RhyaSingerDataVO rhyaSingerDataVO = subscribeList.get(0);
                subscribeIndex.setText("1");
                subscribeName.setText(rhyaSingerDataVO.getName());
            }else {
                subscribeIndex.setText(noSubscribeText);
                subscribeName.setText("");
            }
        }else {
            subscribeIndex.setText(noSubscribeText);
            subscribeName.setText("");
        }
        if (isAdded()) {
            if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                ((ActivityMain) requireActivity()).playerService.subscribeIndex = 1;
            }
        }


        subscribeLayout.setOnClickListener(v -> {
            if (!subscribeName.getText().toString().equals(noSubscribeText)) {
                ((ActivityMain) requireActivity()).showTaskDialog();
                ((ActivityMain) requireActivity()).showSearchResultFragment(subscribeName.getText().toString());
            }
        });


        if (searchTopSongFragment1 == null) searchTopSongFragment1 = SearchTopSongFragment.newInstance(1);
        if (searchTopSongFragment2 == null) searchTopSongFragment2 = SearchTopSongFragment.newInstance(5);


        searchTextView.setOnClickListener(v -> showSearchInputFragment());
        searchImageView.setOnClickListener(v -> showSearchInputFragment());
        _view.setOnClickListener(v -> showSearchInputFragment());


        // 재생수 Top 노래 Viewpager
        RhyaSearchTopMusicAdapter rhyaSearchTopMusicAdapter = new RhyaSearchTopMusicAdapter(getChildFragmentManager(), getLifecycle());
        rhyaSearchTopMusicAdapter.addFragment(searchTopSongFragment1);
        rhyaSearchTopMusicAdapter.addFragment(searchTopSongFragment2);
        viewPager2.setAdapter(rhyaSearchTopMusicAdapter);
        //인디케이터 타입1
        WormDotsIndicator dotsIndicator = view.findViewById(R.id.dots_indicator);
        dotsIndicator.setViewPager2(viewPager2);

        stickyScrollView.setScrollViewListener(new IScrollViewListener() {
            @Override
            public void onScrollChanged(int l, int t, int oldl, int oldt) {
                if (t == 0) {
                    viewLayout.setVisibility(View.INVISIBLE);
                }else {
                    viewLayout.setVisibility(View.VISIBLE);
                }
            }

            @Override
            public void onScrollStopped(boolean isStoped) {

            }
        });


        // 노래 분위기 종류
        // ---------------------------------------------------------- //
        // ---------------------------------------------------------- //
        Button songTypeBtn1 = view.findViewById(R.id.btn1);
        Button songTypeBtn2 = view.findViewById(R.id.btn2);
        songTypeBtn1.setText("#신남");
        songTypeBtn2.setText("#슬픔");
        songTypeBtn1.setOnClickListener(v -> rhyaDialogManager.createDialog_SongTypeList(requireContext(), this, songTypeBtn1.getText().toString(), true));
        songTypeBtn2.setOnClickListener(v -> rhyaDialogManager.createDialog_SongTypeList(requireContext(), this, songTypeBtn2.getText().toString(), true));
        // ---------------------------------------------------------- //
        Button songTypeBtn3 = view.findViewById(R.id.btn3);
        Button songTypeBtn4 = view.findViewById(R.id.btn4);
        songTypeBtn3.setText("#즐거움");
        songTypeBtn4.setText("#잔잔");
        songTypeBtn3.setOnClickListener(v -> rhyaDialogManager.createDialog_SongTypeList(requireContext(), this, songTypeBtn3.getText().toString(), true));
        songTypeBtn4.setOnClickListener(v -> rhyaDialogManager.createDialog_SongTypeList(requireContext(), this, songTypeBtn4.getText().toString(), true));
        // ---------------------------------------------------------- //
        Button songTypeBtn5 = view.findViewById(R.id.btn5);
        songTypeBtn5.setText("#모음집");
        songTypeBtn5.setOnClickListener(v -> rhyaDialogManager.createDialog_SongTypeList(requireContext(), this, songTypeBtn5.getText().toString(), true));
        // ---------------------------------------------------------- //
        // ---------------------------------------------------------- //
    }



    public boolean isActivityCheck() {
        if (isAdded()) {
            return subscribeIndex != null && subscribeName != null && subscribeLayout != null;
        }

        return false;
    }



    public void setSubscribeList(int index) {
        final int setInt = index;
        final String noSubscribeText = "구독 중인 사람 없음";

        if (!isActivityCheck()) {
            return;
        }
        if (index == -1) {
            subscribeIndex.setText(noSubscribeText);
            subscribeName.setText("");
        }

        Animation animation1 = AnimationUtils.loadAnimation(requireContext(), R.anim.anim_subscribe_down_to_up);
        Animation animation2 = AnimationUtils.loadAnimation(requireContext(), R.anim.anim_subscribe_up_to_down);

        animation1.setAnimationListener(new Animation.AnimationListener() {
            @Override
            public void onAnimationStart(Animation animation) {

            }

            @Override
            public void onAnimationEnd(Animation animation) {
                // 구독자 리스트 표시
                ArrayList<RhyaSingerDataVO> subscribeList = RhyaApplication.rhyaMusicDataVO.getSubscribeList();
                if (subscribeList != null) {
                    if (subscribeList.size() != 0) {
                        int showInt = setInt;
                        if (subscribeList.size() - 1 < setInt) showInt = 0;

                        RhyaSingerDataVO rhyaSingerDataVO = subscribeList.get(showInt);
                        subscribeIndex.setText(String.valueOf(showInt + 1));
                        subscribeName.setText(rhyaSingerDataVO.getName());
                    }else {
                        subscribeIndex.setText(noSubscribeText);
                        subscribeName.setText("");
                    }
                }else {
                    subscribeIndex.setText(noSubscribeText);
                    subscribeName.setText("");
                }

                subscribeLayout.startAnimation(animation2);
            }

            @Override
            public void onAnimationRepeat(Animation animation) {

            }
        });
        animation2.setAnimationListener(new Animation.AnimationListener() {
            @Override
            public void onAnimationStart(Animation animation) {

            }

            @Override
            public void onAnimationEnd(Animation animation) {

            }

            @Override
            public void onAnimationRepeat(Animation animation) {

            }
        });

        subscribeLayout.startAnimation(animation1);
    }



    private void showSearchInputFragment() {
        ((ActivityMain) requireActivity()).showSearchInputFragment();
    }
}