package kro.kr.rhya_network.utaiteplayer.fragment;

import android.app.Activity;
import android.app.Dialog;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;

import com.pnikosis.materialishprogress.ProgressWheel;

import java.util.ArrayList;
import java.util.UUID;


import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaNowPlayListAdapter;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVOv2;


/**
 * A simple {@link Fragment} subclass.
 * create an instance of this fragment.
 */
public class NowPlayListFragment extends Fragment {
    public NowPlayListFragment() {
        // Required empty public constructor
    }

    public RhyaNowPlayListAdapter rhyaNowPlayListAdapter = null;

    public TextView noPlaySongTitle;
    public TextView textView;
    public RecyclerView recyclerView;
    public ProgressWheel progressWheel;

    public static NowPlayListFragment newInstance() {
        return new NowPlayListFragment();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_now_play_list, container, false);
    }


    @Override
    public void onDestroy() {
        super.onDestroy();
    }


    public void loadData(boolean isActivityMain) {
        if (textView == null || recyclerView == null || progressWheel == null || noPlaySongTitle == null) return;
        new RhyaAsyncTask<String, String>() {
            private int playPos = -1;
            private ArrayList<RhyaMusicInfoVOv2> rhyaMusicInfoVOArrayList;

            @Override
            protected void onPreExecute() {
                StringBuilder sb = new StringBuilder();
                sb.append(RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size());
                sb.append(" / 100");

                textView.setText(sb.toString());

                recyclerView.setVisibility(View.INVISIBLE);
                progressWheel.setVisibility(View.VISIBLE);
                noPlaySongTitle.setVisibility(View.INVISIBLE);
                rhyaMusicInfoVOArrayList = new ArrayList<>();
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() == 0) return "nullList";

                    try {
                        for (int i = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() - 1; i >= 0; i--) {
                            String uuid = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(i);
                            RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);
                            assert rhyaMusicInfoVO != null;

                            boolean isPlay = false;
                            if (isAdded()) {
                                if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                                    String nowPlayUUID;
                                    int nowPlayIndex;

                                    nowPlayUUID = ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID;
                                    nowPlayIndex = ((ActivityMain) requireActivity()).playerService.playMusicIndex;

                                    if (nowPlayIndex != -1 && nowPlayUUID != null) {
                                        if (nowPlayUUID.equals(rhyaMusicInfoVO.getUuid()) && i == nowPlayIndex) {
                                            isPlay = true;

                                            playPos = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() - 1 - i;
                                        }
                                    }
                                }
                            }

                            UUID uuidGen = UUID.randomUUID();

                            RhyaMusicInfoVOv2 rhyaMusicInfoVOv2 = new RhyaMusicInfoVOv2(rhyaMusicInfoVO.getUuid(),
                                    rhyaMusicInfoVO.getName(),
                                    rhyaMusicInfoVO.getTime(),
                                    rhyaMusicInfoVO.getLyrics(),
                                    rhyaMusicInfoVO.getSinger(),
                                    rhyaMusicInfoVO.getSingerUuid(),
                                    rhyaMusicInfoVO.getSingerImage(),
                                    rhyaMusicInfoVO.getSongWriter(),
                                    rhyaMusicInfoVO.getImage(),
                                    rhyaMusicInfoVO.getMp3(),
                                    rhyaMusicInfoVO.getType(),
                                    rhyaMusicInfoVO.getDate(),
                                    rhyaMusicInfoVO.getVersion(),
                                    isPlay,
                                    uuidGen.toString(),
                                    i,
                                    -1);

                            rhyaMusicInfoVOArrayList.add(rhyaMusicInfoVOv2);
                        }

                        return "success";
                    }catch (Exception ex) {
                        ex.printStackTrace();

                        if (isAdded()) {
                            requireActivity().runOnUiThread(() -> ((ActivityMain) requireActivity()).rhyaDialogManager.createDialog_Confirm(requireContext(),
                                    "Unknown Error",
                                    "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00048)",
                                    "닫기",
                                    true,
                                    Dialog::dismiss));
                        }
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    if (isAdded()) {
                        requireActivity().runOnUiThread(() -> ((ActivityMain) requireActivity()).rhyaDialogManager.createDialog_Confirm(requireContext(),
                                "Unknown Error",
                                "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00050)",
                                "닫기",
                                true,
                                Dialog::dismiss));
                    }
                }

                return null;
            }

            @Override
            protected void onPostExecute(String result) {
                try {
                    if (result != null) {
                        if (result.equals("success")) {
                            rhyaNowPlayListAdapter.setList(rhyaMusicInfoVOArrayList);
                            recyclerView.setVisibility(View.VISIBLE);

                            if (isActivityMain) {
                                try {
                                    int pos = playPos;
                                    new Handler().postDelayed(() -> {
                                        try {
                                            recyclerView.smoothScrollToPosition(pos);
                                        }catch (Exception ignored) {}
                                    }, 200); //sometime not working, need some delay
                                }catch (Exception ex) {
                                    ex.printStackTrace();
                                }
                            }
                        }else {
                            // 재생 목록 없음
                            noPlaySongTitle.setVisibility(View.VISIBLE);
                            recyclerView.setVisibility(View.GONE);
                        }

                        progressWheel.setVisibility(View.GONE);
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    if (isAdded()) {
                        Toast.makeText(requireContext(), "알 수 없는 오류가 발생하였습니다. (00051)", Toast.LENGTH_SHORT).show();
                    }
                }

                Activity activity = getActivity();
                if(activity != null) ((ActivityMain) activity).dismissTaskDialog();
            }
        }.execute(null);
    }


    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        noPlaySongTitle = view.findViewById(R.id.noPlaySongTitle);
        progressWheel = view.findViewById(R.id.progressWheel);
        textView = view.findViewById(R.id.myCountTextView);
        recyclerView = view.findViewById(R.id.recyclerView);

        LinearLayoutManager linearLayoutManager = new LinearLayoutManager(requireContext());

        recyclerView.setLayoutManager(linearLayoutManager);

        rhyaNowPlayListAdapter = new RhyaNowPlayListAdapter(this);

        progressWheel = view.findViewById(R.id.progressWheel);
        recyclerView.setAdapter(rhyaNowPlayListAdapter);


        loadData(true);
    }
}