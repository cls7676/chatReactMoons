// Copyright (c) Microsoft. All rights reserved.

import {
    Body1,
    Button,
    Menu,
    MenuItem,
    MenuList,
    MenuPopover,
    MenuTrigger,
    Spinner,
    Subtitle1,
    Title3
} from '@fluentui/react-components';
import {
    Book24Regular,
    CheckmarkCircle24Regular,
    Code24Regular,
    PlayCircle24Regular,
    Thinking24Regular
} from '@fluentui/react-icons';
import { FC, useEffect, useState } from 'react';
import { useSemanticKernel } from '../hooks/useSemanticKernel';
import { IAsk, IAskInput } from '../model/Ask';
import { IKeyConfig } from '../model/KeyConfig';
import { IBookResult, IPage } from '../model/Page';

interface IData {
    uri: string;
    title: string;
    description: string;
    keyConfig: IKeyConfig;
    onBack: () => void;
}

interface IProcessHistory {
    uri: string;
    functionName: string;
    input: string;
    timestamp: string;
}

enum BookCreationState {
    Ready = 0,
    GetNovelOutline = 1,
    GetSummaryOfOutline = 2,
    ReadyToCreateBookFromOutline = 3,
    CreateBookFromOutline = 4,
    Complete = 5,
}

const CreateBook: FC<IData> = ({ uri, title, description, keyConfig, onBack }) => {
    const sk = useSemanticKernel(uri);
    const [bookCreationState, setBookCreationState] = useState<BookCreationState>(BookCreationState.Ready);
    const [busyMessage, setBusyMessage] = useState<string>('');
    const [bookState, setBookState] = useState<IBookResult>({} as IBookResult);
    const [showProcess, setShowProcess] = useState<boolean>(false);
    const [processHistory, setProcessHistory] = useState<IProcessHistory[]>([]);

    useEffect(() => {
        const runAsync = async () => {
            switch (bookCreationState) {
                case BookCreationState.GetNovelOutline:
                    setBusyMessage('Creating novel outline');
                    await runNovelOutlineFunction();
                    setBusyMessage('');
                    break;
                case BookCreationState.GetSummaryOfOutline:
                    setBusyMessage('Getting summary');
                    await runSummariseFunction();
                    setBusyMessage('');
                    break;
                case BookCreationState.CreateBookFromOutline:
                    setBusyMessage('Creating an 8 page book');
                    await runCreateBookFunction();
                    setBusyMessage('');
                    break;
            }
        };

        runAsync();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [bookCreationState]);

    useEffect(() => {
        switch (bookCreationState) {
            case BookCreationState.GetNovelOutline:
                setBookCreationState(BookCreationState.GetSummaryOfOutline);
                break;
            case BookCreationState.GetSummaryOfOutline:
                setBookCreationState(BookCreationState.ReadyToCreateBookFromOutline);
                break;
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [bookState]);

    const runNovelOutlineFunction = async () => {
        var askInputs: IAskInput[] = [
            {
                key: 'chapterCount',
                value: '2',
            },
            {
                key: 'endMarker',
                value: '<hr />',
            },
        ];

        var ask: IAsk = { value: `A children's book called ${title} about ${description}`, inputs: askInputs };

        try {
            var result = await sk.invokeAsync(keyConfig, ask, 'writerskill', 'noveloutline');
            var historyItem = {
                functionName: 'noveloutline',
                input: JSON.stringify(ask),
                timestamp: new Date().toTimeString(),
                uri: '/api/skills/writerskill/invoke/noveloutline',
            };
            setProcessHistory((processHistory) => [...processHistory, historyItem]);

            setBookState((bookState) => ({
                ...bookState,
                outline: (result.value as string).substring(0, (result.value as string).length),
            }));
        } catch (e) {
            alert('Something went wrong.\n\nDetails:\n' + e);
        }
    };
    const runCreateBookFunction = async () => {
        var inputs: IAskInput[] = [
            {
                key: 'numPages',
                value: '8',
            },
            {
                key: 'numWordsPerPage',
                value: '50',
            },
        ];

        var ask: IAsk = { value: bookState.outline, inputs: inputs };

        try {
            var result = await sk.invokeAsync(keyConfig, ask, 'childrensbookskill', 'createbook');

            var historyItem = {
                functionName: 'createbook',
                input: JSON.stringify(ask),
                timestamp: new Date().toTimeString(),
                uri: '/api/skills/childrensbookskill/invoke/createbook',
            };
            setProcessHistory((processHistory) => [...processHistory, historyItem]);

            var jsonValue = (result.value as string).substring((result.value as string).indexOf('['));

            var results = JSON.parse(jsonValue);

            var pages: IPage[] = [];

 