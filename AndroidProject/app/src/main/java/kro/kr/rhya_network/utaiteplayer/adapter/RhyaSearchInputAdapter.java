package kro.kr.rhya_network.utaiteplayer.adapter;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.text.Spannable;
import android.text.SpannableString;
import android.text.style.ForegroundColorSpan;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.fragment.SearchInputFragment;

public class RhyaSearchInputAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    private final Activity activity;
    private Context context;
    private String searchText;
    private ArrayList<String> searchList = null;


    public RhyaSearchInputAdapter(Activity activity) {
        this.activity = activity;
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_search, parent, false);
        context = view.getContext();

        return new RhyaSearchInputAdapter.ViewHolder(view);
    }

    @SuppressLint("NotifyDataSetChanged")
    public void setList(ArrayList<String> list, String search) {
        this.searchList = list;
        searchText = search;


        notifyDataSetChanged();
    }

    @Override
    public int getItemCount() {
        return searchList == null ? 0 : searchList.size();
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        ((RhyaSearchInputAdapter.ViewHolder) holder).onBind(searchList.get(position));
    }


    private class ViewHolder extends RecyclerView.ViewHolder {
        private final TextView searchName;

        public ViewHolder(@NonNull View itemView) {
            super(itemView);

            searchName = itemView.findViewById(R.id.textView);
            itemView.setOnClickListener(v -> {
                int pos = getBindingAdapterPosition();
                if (pos != RecyclerView.NO_POSITION) {
                    String input = searchList.get(pos);

                    RhyaApplication.searchText = input;
                    ((SearchInputFragment) (((ActivityMain) activity).searchInputFragment)).setSearchText(input);
                    ((ActivityMain) activity).showSearchResultFragment(input);
                }
            });
        }

        public void onBind(String value) {
            if (value.contains(searchText)) {
                SpannableString spannableString = new SpannableString(value);
                int start = value.indexOf(searchText);
                int end = start + searchText.length();
                spannableString.setSpan(new ForegroundColorSpan(ContextCompat.getColor(context, R.color.app_main_base_6)), start, end, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                searchName.setText(spannableString);
            } else {
                searchName.setText(value);
            }
        }
    }
}