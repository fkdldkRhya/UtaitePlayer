package kro.kr.rhya_network.utaiteplayer.fragment;

import android.app.Activity;
import android.content.ContentValues;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import com.bumptech.glide.Glide;
import com.bumptech.glide.signature.ObjectKey;
import com.github.siyamed.shapeimageview.RoundedImageView;

import org.json.JSONObject;

import java.util.Iterator;
import java.util.Objects;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpsConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link SearchTopSongFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class SearchTopSongFragment extends Fragment {
    private int startIndex = 1;

    // UI Object
    private TextView rank1 = null;
    private ImageButton playBtn1 = null;
    private ImageButton moreBtn1 = null;
    private TextView songName1 = null;
    private TextView songSinger1 = null;
    private RoundedImageView songImage1 = null;
    private TextView rank2 = null;
    private ImageButton playBtn2 = null;
    private ImageButton moreBtn2 = null;
    private TextView songName2 = null;
    private TextView songSinger2 = null;
    private RoundedImageView songImage2 = null;
    private TextView rank3 = null;
    private ImageButton playBtn3 = null;
    private ImageButton moreBtn3 = null;
    private TextView songName3 = null;
    private TextView songSinger3 = null;
    private RoundedImageView songImage3 = null;
    private TextView rank4 = null;
    private ImageButton playBtn4 = null;
    private ImageButton moreBtn4 = null;
    private TextView songName4 = null;
    private TextView songSinger4 = null;
    private RoundedImageView songImage4 = null;



    public SearchTopSongFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @return A new instance of fragment SearchTopSongFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static SearchTopSongFragment newInstance(int index) {
        SearchTopSongFragment searchTopSongFragment = new SearchTopSongFragment();
        Bundle bundle = new Bundle();
        bundle.putInt("index", index);

        searchTopSongFragment.setArguments(bundle);

        return searchTopSongFragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        assert getArguments() != null;
        startIndex = getArguments().getInt("index");

        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_search_topsong, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);



        rank1 = view.findViewById(R.id.rank1);
        songName1 = view.findViewById(R.id.name1);
        songSinger1 = view.findViewById(R.id.singer1);
        songImage1 = view.findViewById(R.id.imageView1);
        playBtn1 = view.findViewById(R.id.playButton1);
        moreBtn1 = view.findViewById(R.id.moreButton1);


        rank2 = view.findViewById(R.id.rank2);
        songName2 = view.findViewById(R.id.name2);
        songSinger2 = view.findViewById(R.id.singer2);
        songImage2 = view.findViewById(R.id.imageView2);
        playBtn2 = view.findViewById(R.id.playButton2);
        moreBtn2 = view.findViewById(R.id.moreButton2);


        rank3 = view.findViewById(R.id.rank3);
        songName3 = view.findViewById(R.id.name3);
        songSinger3 = view.findViewById(R.id.singer3);
        songImage3 = view.findViewById(R.id.imageView3);
        playBtn3 = view.findViewById(R.id.playButton3);
        moreBtn3 = view.findViewById(R.id.moreButton3);


        rank4 = view.findViewById(R.id.rank4);
        songName4 = view.findViewById(R.id.name4);
        songSinger4 = view.findViewById(R.id.singer4);
        songImage4 = view.findViewById(R.id.imageView4);
        playBtn4 = view.findViewById(R.id.playButton4);
        moreBtn4 = view.findViewById(R.id.moreButton4);


        // 서버 접속
        new RhyaAsyncTask<String, String>() {
            private RhyaMusicInfoVO[] rhyaMusicInfoVOS;

            @Override
            protected void onPreExecute() {
                assert getParentFragment() != null;
                ((SearchFragment) getParentFragment()).loadImageView.setVisibility(View.VISIBLE);
                ((SearchFragment) getParentFragment()).viewPager2.setVisibility(View.INVISIBLE);

                rhyaMusicInfoVOS = new RhyaMusicInfoVO[8];
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                    ContentValues urlParm = new ContentValues();

                    urlParm.put("mode", "12");
                    urlParm.put("auth", ((ActivityMain) requireActivity()).rhyaCore.getAutoLogin(((ActivityMain) requireActivity()).rhyaSharedPreferences, requireContext()));

                    JSONObject jsonObjectRoot = new JSONObject(rhyaHttpsConnection.request(((ActivityMain) requireActivity()).rhyaCore.MAIN_URL, urlParm));

                    for (Iterator<String> iter = jsonObjectRoot.keys(); iter.hasNext(); ) {
                        String key = iter.next();
                        JSONObject jsonObject = new JSONObject(jsonObjectRoot.getString(key));

                        if (!RhyaApplication.rhyaMusicInfoVOHashMap.containsKey(jsonObject.getString("uuid"))) {
                            continue;
                        }

                        RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(jsonObject.getString("uuid"));

                        rhyaMusicInfoVOS[Integer.parseInt(key) - 1] = rhyaMusicInfoVO;
                    }

                    return "success";
                }catch (Exception ex) {
                    ex.printStackTrace();
                }

                return null;
            }

            @Override
            protected void onPostExecute(String result) {
                if (result != null) {
                    assert getParentFragment() != null;
                    ((SearchFragment) getParentFragment()).loadImageView.setVisibility(View.GONE);
                    ((SearchFragment) getParentFragment()).progressWheel.setVisibility(View.GONE);
                    ((SearchFragment) getParentFragment()).viewPager2.setVisibility(View.VISIBLE);

                    try {
                        RhyaMusicInfoVO rhyaMusicInfoVO;

                        rhyaMusicInfoVO = rhyaMusicInfoVOS[startIndex - 1];
                        rank1.setText(String.valueOf(startIndex));
                        songName1.setText(rhyaMusicInfoVO.getName());
                        songSinger1.setText(rhyaMusicInfoVO.getSinger());
                        Glide.with(requireActivity())
                                .load(rhyaMusicInfoVO.getImage())
                                .placeholder(R.drawable.img_load_error)
                                .error(R.drawable.img_load_error)
                                .fallback(R.drawable.img_load_error)
                                .signature(new ObjectKey(rhyaMusicInfoVO.getVersion()))
                                .into(songImage1);
                        final RhyaMusicInfoVO finalRhyaMusicInfoVO1 = rhyaMusicInfoVO;
                        playBtn1.setOnClickListener(v -> runCommand(finalRhyaMusicInfoVO1));
                        moreBtn1.setOnClickListener(v -> ((ActivityMain) requireActivity()).showMorePopupMenu(moreBtn1, finalRhyaMusicInfoVO1.getUuid(), null, -1, 1));

                        startIndex = startIndex + 1;

                        rhyaMusicInfoVO = rhyaMusicInfoVOS[startIndex - 1];
                        rank2.setText(String.valueOf(startIndex));
                        songName2.setText(rhyaMusicInfoVO.getName());
                        songSinger2.setText(rhyaMusicInfoVO.getSinger());
                        Glide.with(requireActivity())
                                .load(rhyaMusicInfoVO.getImage())
                                .placeholder(R.drawable.img_load_error)
                                .error(R.drawable.img_load_error)
                                .fallback(R.drawable.img_load_error)
                                .signature(new ObjectKey(rhyaMusicInfoVO.getVersion()))
                                .into(songImage2);
                        final RhyaMusicInfoVO finalRhyaMusicInfoVO2 = rhyaMusicInfoVO;
                        playBtn2.setOnClickListener(v -> runCommand(finalRhyaMusicInfoVO2));
                        moreBtn2.setOnClickListener(v -> ((ActivityMain) requireActivity()).showMorePopupMenu(moreBtn2, finalRhyaMusicInfoVO2.getUuid(), null, -1, 1));

                        startIndex = startIndex + 1;

                        rhyaMusicInfoVO = rhyaMusicInfoVOS[startIndex - 1];
                        rank3.setText(String.valueOf(startIndex));
                        songName3.setText(rhyaMusicInfoVO.getName());
                        songSinger3.setText(rhyaMusicInfoVO.getSinger());
                        Glide.with(requireActivity())
                                .load(rhyaMusicInfoVO.getImage())
                                .placeholder(R.drawable.img_load_error)
                                .error(R.drawable.img_load_error)
                                .fallback(R.drawable.img_load_error)
                                .signature(new ObjectKey(rhyaMusicInfoVO.getVersion()))
                                .into(songImage3);
                        final RhyaMusicInfoVO finalRhyaMusicInfoVO3 = rhyaMusicInfoVO;
                        playBtn3.setOnClickListener(v -> runCommand(finalRhyaMusicInfoVO3));
                        moreBtn3.setOnClickListener(v -> ((ActivityMain) requireActivity()).showMorePopupMenu(moreBtn3, finalRhyaMusicInfoVO3.getUuid(), null, -1, 1));

                        startIndex = startIndex + 1;

                        rhyaMusicInfoVO = rhyaMusicInfoVOS[startIndex - 1];
                        rank4.setText(String.valueOf(startIndex));
                        songName4.setText(rhyaMusicInfoVO.getName());
                        songSinger4.setText(rhyaMusicInfoVO.getSinger());
                        Glide.with(requireActivity())
                                .load(rhyaMusicInfoVO.getImage())
                                .placeholder(R.drawable.img_load_error)
                                .error(R.drawable.img_load_error)
                                .fallback(R.drawable.img_load_error)
                                .signature(new ObjectKey(rhyaMusicInfoVO.getVersion()))
                                .into(songImage4);
                        final RhyaMusicInfoVO finalRhyaMusicInfoVO4 = rhyaMusicInfoVO;
                        playBtn4.setOnClickListener(v -> runCommand(finalRhyaMusicInfoVO4));
                        moreBtn4.setOnClickListener(v -> ((ActivityMain) requireActivity()).showMorePopupMenu(moreBtn4, finalRhyaMusicInfoVO4.getUuid(), null, -1, 1));
                    }catch (Exception ex) {
                        ex.printStackTrace();
                    }
                }
            }
        }.execute(null);
    }


    // 노래 재생
    private void runCommand(RhyaMusicInfoVO item) {
        try {
            ((ActivityMain) requireActivity()).putMusicUUID(item.getUuid());
        }catch (Exception ex) {
            ex.printStackTrace();

            Toast.makeText(requireActivity().getApplicationContext(), "알 수 없는 오류가 발생하였습니다. (00075)", Toast.LENGTH_SHORT).show();
        }
    }
}