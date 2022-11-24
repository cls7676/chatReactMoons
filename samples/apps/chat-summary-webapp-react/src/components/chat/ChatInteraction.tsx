// Copyright (c) Microsoft. All rights reserved.

import { Body1, Button, Title3 } from '@fluentui/react-components';
import React, { FC, useState } from 'react';
import { ChatHistoryItem } from './ChatHistoryItem';
import { ChatInput } from './ChatInput';
import { ChatThread, IChatMessage } from './ChatThread';

interface IData {
    uri: string;
    onGetAISummary: (chat: IChatMessage[]) => void;
    onBack: () => void;
}

const ChatInteraction: FC<IData> = ({ uri, onGetAISummary, onBack }) => {
    const chatBottomRef = React.useRef<HTMLDivElement>(null);
    const [isBusy, setIsBusy] = useState<boolean>();
    const [chatHistory, setChatHistory] = useState<IChatMessage[]>(ChatThread);

    React.useEffect(() => {
        chatBottomRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [isBusy]);

    return (
        <div style={{ paddingTop: 20, gap: 20, display: 'flex', flexDirection: 'column', alignItems: 'stretch' }}>
            <Title3 style={{ alignItems: 'left' }}>Interact with Chat</Title3>
            <Body1>
                This is not a