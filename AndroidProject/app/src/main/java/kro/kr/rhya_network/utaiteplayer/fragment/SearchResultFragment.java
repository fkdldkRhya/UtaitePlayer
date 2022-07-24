package kro.kr.rhya_network.utaiteplayer.fragment;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.widget.NestedScrollView;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import java.util.ArrayList;
import java.util.HashMap;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaSearchResultLyricsAdapter;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaSearchResultSingerAdapter;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaSearchResultSongAdapter;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaDialogManagerNew;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaGetMusicLyrics;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHangulInitialSearch;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;

public class SearchResultFragment extends Fragment {
    private TextView searchTextView;
    private View viewLayout;
    private TextView noItemTextViewForSong;
    private TextView noItemTextViewForSinger;
    private TextView noItemTextViewForLyrics;
    private TextView allViewButtonForSong;
    private TextView allViewButtonForSinger;
    private TextView allViewButtonForLyrics;


    private RhyaDialogManagerNew rhyaDialogManagerNew_1;
    private RhyaDialogManagerNew rhyaDialogManagerNew_2;
    private RhyaDialogManagerNew rhyaDialogManagerNew_3;
    private RhyaSearchResultSongAdapter rhyaSearchResultSongAdapter;
    private RhyaSearchResultSingerAdapter rhyaSearchResultSingerAdapter;
    private RhyaSearchResultLyricsAdapter rhyaSearchResultLyricsAdapter;




    public SearchResultFragment() {
        // Required empty public constructor
    }




    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @return A new instance of fragment SearchFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static SearchResultFragment newInstance() {
        return new SearchResultFragment();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_search_result, container, false);
    }



    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        viewLayout = view.findViewById(R.id.viewSelector);
        noItemTextViewForSong = view.findViewById(R.id.noItemForSong);
        noItemTextViewForSinger = view.findViewById(R.id.noItemForSinger);
        noItemTextViewForLyrics = view.findViewById(R.id.noItemForLyrics);
        allViewButtonForSong = view.findViewById(R.id.allViewButtonForSong);
        allViewButtonForSinger = view.findViewById(R.id.allViewButtonForSinger);
        allViewButtonForLyrics = view.findViewById(R.id.allViewButtonForLyrics);
        RecyclerView recyclerViewForSong = view.findViewById(R.id.songRecyclerView);
        RecyclerView recyclerViewForSinger = view.findViewById(R.id.singerRecyclerView);
        RecyclerView recyclerViewForLyrics = view.findViewById(R.id.lyricsRecyclerView);
        NestedScrollView nestedScrollView = view.findViewById(R.id.nestedScrollView);
        nestedScrollView.setOnScrollChangeListener((NestedScrollView.OnScrollChangeListener) (v, scrollX, scrollY, oldScrollX, oldScrollY) -> {
            if (scrollY == 0) {
                viewLayout.setVisibility(View.INVISIBLE);
            }else {
                viewLayout.setVisibility(View.VISIBLE);
            }
        });

        rhyaDialogManagerNew_1 = new RhyaDialogManagerNew(requireActivity(), true, 1);
        rhyaDialogManagerNew_2 = new RhyaDialogManagerNew(requireActivity(), true, 2);
        rhyaDialogManagerNew_3 = new RhyaDialogManagerNew(requireActivity(), true, 3);

        rhyaSearchResultSongAdapter = new RhyaSearchResultSongAdapter(requireActivity());
        rhyaSearchResultSingerAdapter = new RhyaSearchResultSingerAdapter(requireActivity());
        rhyaSearchResultLyricsAdapter = new RhyaSearchResultLyricsAdapter(requireActivity());
        LinearLayoutManager recyclerViewForSongLinearLayoutManager = new LinearLayoutManager(getActivity());
        recyclerViewForSongLinearLayoutManager.setOrientation(RecyclerView.VERTICAL);
        recyclerViewForSong.setAdapter(rhyaSearchResultSongAdapter);
        recyclerViewForSong.setLayoutManager(recyclerViewForSongLinearLayoutManager);
        LinearLayoutManager recyclerViewForSingerLinearLayoutManager = new LinearLayoutManager(getActivity());
        recyclerViewForSingerLinearLayoutManager.setOrientation(RecyclerView.VERTICAL);
        recyclerViewForSinger.setAdapter(rhyaSearchResultSingerAdapter);
        recyclerViewForSinger.setLayoutManager(recyclerViewForSingerLinearLayoutManager);
        LinearLayoutManager recyclerViewForLyricsLinearLayoutManager = new LinearLayoutManager(getActivity());
        recyclerViewForLyricsLinearLayoutManager.setOrientation(RecyclerView.VERTICAL);
        recyclerViewForLyrics.setAdapter(rhyaSearchResultLyricsAdapter);
        recyclerViewForLyrics.setLayoutManager(recyclerViewForLyricsLinearLayoutManager);


        // ====================================================================
        // Back search fragment
        // ====================================================================
        searchTextView = view.findViewById(R.id.searchTextView);
        ImageView searchImageView = view.findViewById(R.id.searchImageView);
        View _view = view.findViewById(R.id.view);
        searchTextView.setOnClickListener(v -> ((ActivityMain) requireActivity()).showSearchInputFragment());
        searchImageView.setOnClickListener(v -> ((ActivityMain) requireActivity()).showSearchInputFragment());
        _view.setOnClickListener(v -> ((ActivityMain) requireActivity()).showSearchInputFragment());
        // ====================================================================
        // ====================================================================
    }


    @Override
    public void onResume() {
        super.onResume();


        ((ActivityMain) requireActivity()).showTaskDialog();

        searchTextView.setText(RhyaApplication.searchText);

        // 데이터 검색
        new RhyaAsyncTask<String, String>() {
            private ArrayList<String> arrayListForSong;
            private ArrayList<String> arrayListForSinger;
            private HashMap<String, ArrayList<RhyaMusicInfoVO>> stringRhyaMusicInfoVOHashMap;
            private ArrayList<RhyaMusicInfoVO> arrayListForLyrics;


            @Override
            protected void onPreExecute() {
                arrayListForSong = new ArrayList<>();
                arrayListForSinger = new ArrayList<>();
                stringRhyaMusicInfoVOHashMap = new HashMap<>();
                arrayListForLyrics = new ArrayList<>();
                rhyaSearchResultSongAdapter.setList(arrayListForSong);
                rhyaSearchResultSingerAdapter.setList(arrayListForSinger, stringRhyaMusicInfoVOHashMap, true);
                rhyaSearchResultLyricsAdapter.setList(arrayListForLyrics, true);
            }

            @Override
            protected String doInBackground(String arg) {
                RhyaHangulInitialSearch rhyaHangulInitialSearch = new RhyaHangulInitialSearch();

                try {
                    Thread.sleep(500);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }

                for (String uuid : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
                    try {
                        RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);
                        assert rhyaMusicInfoVO != null;

                        arg = arg.toLowerCase();

                        String songName = rhyaMusicInfoVO.getName().toLowerCase();
                        String singerName = rhyaMusicInfoVO.getSinger().toLowerCase();
                        String writerName = rhyaMusicInfoVO.getSongWriter().toLowerCase();


                        if (    // 곡
                                        rhyaMusicInfoVO.getName().equalsIgnoreCase(arg) ||
                                        rhyaHangulInitialSearch.matchString(songName, arg) ||
                                        songName.contains(arg) ||

                                        // 아티스트
                                        rhyaMusicInfoVO.getSinger().equalsIgnoreCase(arg) ||
                                        rhyaHangulInitialSearch.matchString(singerName, arg) ||
                                        singerName.contains(arg) ||

                                        // 작곡가
                                        rhyaMusicInfoVO.getSongWriter().equalsIgnoreCase(arg) ||
                                        rhyaHangulInitialSearch.matchString(writerName, arg) ||
                                        writerName.contains(arg)
                        ) {
                            if (!arrayListForSong.contains(rhyaMusicInfoVO.getUuid()))
                                arrayListForSong.add(rhyaMusicInfoVO.getUuid());
                        }


                        if (
                                        rhyaMusicInfoVO.getSinger().equalsIgnoreCase(arg) ||
                                        rhyaHangulInitialSearch.matchString(singerName, arg) ||
                                        singerName.contains(arg)
                        ) {
                            if (!arrayListForSinger.contains(rhyaMusicInfoVO.getSingerUuid())) {
                                arrayListForSinger.add(rhyaMusicInfoVO.getSingerUuid());

                                ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOArrayList = new ArrayList<>();
                                for (String uuidSub : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
                                    RhyaMusicInfoVO rhyaMusicInfoVOSub = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuidSub);

                                    assert rhyaMusicInfoVOSub != null;
                                    if (rhyaMusicInfoVOSub.getSingerUuid().equals(rhyaMusicInfoVO.getSingerUuid())) {
                                        rhyaMusicInfoVOArrayList.add(rhyaMusicInfoVOSub);
                                    }
                                }

                                stringRhyaMusicInfoVOHashMap.put(rhyaMusicInfoVO.getSingerUuid(), rhyaMusicInfoVOArrayList);
                            }
                        }


                        String nowLyrics = ((ActivityMain) requireActivity()).rhyaAESManager.aesDecode(((ActivityMain) requireActivity()).rhyaCore.readLyrics(requireActivity(), rhyaMusicInfoVO.getUuid()));
                        if (nowLyrics.contains(arg)) {
                            rhyaMusicInfoVO.setLyrics(new RhyaGetMusicLyrics().getMusicLyrics(RhyaApplication.searchText, nowLyrics));
                            arrayListForLyrics.add(rhyaMusicInfoVO);
                        }
                    }catch (Exception ex) {
                        ex.printStackTrace();
                    }
                }

                return null;
            }

            @Override
            protected void onPostExecute(String result) {
                // 곡 전용
                if (arrayListForSong.size() > 0) {
                    noItemTextViewForSong.setVisibility(View.INVISIBLE);
                    rhyaSearchResultSongAdapter.setList(arrayListForSong);
                }else {
                    noItemTextViewForSong.setVisibility(View.VISIBLE);
                }

                // 아티스트 전용
                if (arrayListForSinger.size() > 0) {
                    noItemTextViewForSinger.setVisibility(View.INVISIBLE);
                    rhyaSearchResultSingerAdapter.setList(arrayListForSinger, stringRhyaMusicInfoVOHashMap, true);
                }else {
                    noItemTextViewForSinger.setVisibility(View.VISIBLE);
                }

                // 아티스트 전용
                if (arrayListForLyrics.size() > 0) {
                    noItemTextViewForLyrics.setVisibility(View.INVISIBLE);
                    rhyaSearchResultLyricsAdapter.setList(arrayListForLyrics, true);
                }else {
                    noItemTextViewForLyrics.setVisibility(View.VISIBLE);
                }


                allViewButtonForSong.setOnClickListener(v -> rhyaDialogManagerNew_1.setAndShowDialogSettingType_1(arrayListForSong));
                allViewButtonForSinger.setOnClickListener(v -> rhyaDialogManagerNew_2.setAndShowDialogSettingType_2(arrayListForSinger, stringRhyaMusicInfoVOHashMap));
                allViewButtonForLyrics.setOnClickListener(v -> rhyaDialogManagerNew_3.setAndShowDialogSettingType_3(arrayListForLyrics));

                ((ActivityMain) requireActivity()).dismissTaskDialog();
            }
        }.execute(RhyaApplication.searchText);
    }
}
